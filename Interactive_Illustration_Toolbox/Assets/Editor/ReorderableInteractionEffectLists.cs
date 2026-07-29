using System;
using System.Collections.Generic;
using Editor;
using InteractionEffects;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


public abstract class ReorderableInteractionEffectLists : UnityEditor.Editor
{
    private struct InteractionEffectTypeReorderableListPair
    {
        public readonly InteractionEffectType effectType;
        public readonly ReorderableList reorderableList;

        public InteractionEffectTypeReorderableListPair(InteractionEffectType effectType,
            ReorderableList reorderableList)
        {
            this.effectType = effectType;
            this.reorderableList = reorderableList;
        }
    }

    protected abstract string[] GetListNames();

    private static GUIStyle guiStyleBoldLabel;
    private static GUIStyle guiStyleRemoveButton;
    private static GUIStyle guiStyleFoldoutButton;
    private static bool stylesSetup;

    private Component targetComponent;

    private static bool currentlyHidingComponents = true;
    private SerializedProperty propertyToDelete = null;

    private List<ReorderableList> reorderableLists = new();
    private ReorderableList currentlyDrawnList;

    protected void OnEnable()
    {
        targetComponent = target as Component;

        string[] listNames = GetListNames();

        foreach (string listName in listNames)
        {
            reorderableLists.Add(CreateReorderableList(serializedObject.FindProperty(listName)));
        }

        if (currentlyHidingComponents)
            HideComponents();
        else
            ShowComponents();
    }

    public override void OnInspectorGUI()
    {
        if (!SetupGuiStyles())
            return;

        serializedObject.Update();

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Hide Components"))
            HideComponents();

        if (GUILayout.Button("Show Components"))
            ShowComponents();

        GUILayout.EndHorizontal();
        GUILayout.Space(10);

        foreach (ReorderableList list in reorderableLists)
        {
            // Have to use a dirty trick by storing a reference to the currently Drawing List to not rewrite Reorderable list footer callback every time.
            currentlyDrawnList = list;
            list.DoLayoutList();

            if (propertyToDelete != null)
            {
                RemovePropertyFromList(propertyToDelete, serializedObject.FindProperty(list.serializedProperty.name));
                propertyToDelete = null;
                return;
            }

            GUILayout.Space(15);
        }

        if (serializedObject.ApplyModifiedProperties())
            EditorUtility.SetDirty(targetComponent);
    }

    /// <summary>
    /// Sets up all gui styles that is needed in this editor and caches them.
    /// </summary>
    /// <returns>True if successful.</returns>
    /// <remarks>This is more performant than creating the GUIStyles every callback.</remarks>
    private static bool SetupGuiStyles()
    {
        if (stylesSetup)
            return stylesSetup;

        guiStyleBoldLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 14
        };

        guiStyleRemoveButton = EditorStyles.miniButton;
        guiStyleRemoveButton.richText = true;
        guiStyleRemoveButton.alignment = TextAnchor.MiddleCenter;
        guiStyleRemoveButton.fontSize = 12;

        guiStyleFoldoutButton = EditorStyles.miniButton;
        guiStyleFoldoutButton.richText = false;
        guiStyleFoldoutButton.alignment = TextAnchor.MiddleCenter;
        guiStyleFoldoutButton.fontSize = 6;

        stylesSetup = true;
        return stylesSetup;
    }

    private void RemovePropertyFromList(SerializedProperty propertyToRemove, SerializedProperty targetList)
    {
        for (int i = 0; i < targetList.arraySize; i++)
        {
            SerializedProperty checkedElement = targetList.GetArrayElementAtIndex(i);

            if (checkedElement.objectReferenceValue != propertyToDelete.objectReferenceValue)
                continue;

            DestroyImmediate(checkedElement.GetObject());
            targetList.DeleteArrayElementAtIndex(i);
            break;
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetComponent);
    }

    private void ShowComponents()
    {
        foreach (ReorderableList list in reorderableLists)
            ChangeVisibilityOfListElements(list.serializedProperty, false);
    }

    private void HideComponents()
    {
        foreach (ReorderableList list in reorderableLists)
            ChangeVisibilityOfListElements(list.serializedProperty, true);
    }

    private static void ChangeVisibilityOfListElements(SerializedProperty list, bool hide)
    {
        for (int i = 0; i < list.arraySize; i++)
        {
            list.GetArrayElementAtIndex(i).GetObject().hideFlags = hide ? HideFlags.HideInInspector : HideFlags.None;
        }

        currentlyHidingComponents = hide;

        // Dirty trick for force refreshing the inspector
        GameObject go = new("GameObject");
        DestroyImmediate(go);
    }

    private void OnInteractionEffectTypeAdded(object obj)
    {
        InteractionEffectTypeReorderableListPair clickInfo = (InteractionEffectTypeReorderableListPair)obj;

        Type effectType = clickInfo.effectType.GetInteractionEffectType();

        InteractionEffect interactionEffect = (InteractionEffect)targetComponent.gameObject.AddComponent(effectType);

        if (currentlyHidingComponents)
            interactionEffect.hideFlags = HideFlags.HideInInspector;

        ReorderableList targetList = clickInfo.reorderableList;
        int index = targetList.serializedProperty.arraySize;
        targetList.serializedProperty.arraySize++;
        targetList.index = index;
        SerializedProperty element = targetList.serializedProperty.GetArrayElementAtIndex(index);
        element.objectReferenceValue = interactionEffect;
        element.isExpanded = true;
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(targetComponent);
    }

    private ReorderableList CreateReorderableList(SerializedProperty targetProperty)
    {
        return new ReorderableList(serializedObject, targetProperty, true, true, false, false)
        {
            drawHeaderCallback = DrawHeaderCallback,
            drawElementCallback = DrawElementCallback,
            elementHeightCallback = ElementHeightCallback,
            drawFooterCallback = DrawFooterCallback
        };
    }

    #region Draw Reorderable List Callbacks

    private void DrawHeaderCallback(Rect rect)
    {
        // Uses currentlyDrawingList that is set in OnGUI since DrawHeaderCallback doesnt include a list reference
        string listName = currentlyDrawnList.serializedProperty.name;
        EditorGUI.LabelField(new Rect(rect.x, rect.y - 1f, rect.width, rect.height),
            char.ToUpper(listName[0]) + listName.Substring(1), guiStyleBoldLabel);
    }

    private void DrawFooterCallback(Rect rect)
    {
        if (!GUI.Button(rect, "+"))
            return;

        GenericMenu menu = new();

        foreach (InteractionEffectType interactionEffectType in Enum.GetValues(typeof(InteractionEffectType)))
            // Uses currentlyDrawingList that is set in OnGUI since DrawFooterCallback doesnt include a list reference
            menu.AddItem(new GUIContent(interactionEffectType.GetMenuParentString() + interactionEffectType),
                false, OnInteractionEffectTypeAdded
                , new InteractionEffectTypeReorderableListPair(interactionEffectType, currentlyDrawnList));

        menu.ShowAsContext();
    }

    private float ElementHeightCallback(int index)
    {
        // Get the elements serialized property
        SerializedProperty element = currentlyDrawnList.serializedProperty.GetArrayElementAtIndex(index);

        // by default we have only the asset reference -> single line
        float height = 0f;

        if (element.objectReferenceValue && element.isExpanded)
        {
            SerializedObject elementSerializedObject = new SerializedObject(element.objectReferenceValue);

            // Find all properties in the referenced script that we want to display and add lines accordingly.
            SerializedProperty sp = elementSerializedObject.GetIterator();
            sp.NextVisible(true);

            while (sp.NextVisible(false))
            {
                height += EditorGUI.GetPropertyHeight(sp, true) + 8;
            }
        }

        return height + (EditorGUIUtility.singleLineHeight + 4);
    }

    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        // Get the element in the list (SerializedProperty)
        SerializedProperty element = currentlyDrawnList.serializedProperty.GetArrayElementAtIndex(index);
        InteractionEffect elementObject = element.GetObject() as InteractionEffect;
        string elementName = elementObject.GetType().Name.ToString();
        // and draw the default object reference field
        EditorGUI.indentLevel++;
        if (!elementObject.CheckSetup()) EditorGUI.DrawRect(rect, new Color(.3f, .1f, .1f, 0.5f));
        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), elementName,
            EditorStyles.boldLabel);

        if (GUI.Button(new Rect(rect.x + EditorStyles.boldLabel.CalcSize(new GUIContent(elementName)).x + 25f, rect.y,
                20,
                EditorGUIUtility.singleLineHeight), "-", guiStyleRemoveButton))
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want to delete " + elementName + "?", "Yes",
                    "No"))
            {
                propertyToDelete = element;
            }
        }

        string buttonText = element.isExpanded ? "▲" : "▼";

        if (GUI.Button(new Rect(rect.width + 20, rect.y, 20, EditorGUIUtility.singleLineHeight), buttonText,
                guiStyleFoldoutButton))
        {
            element.isExpanded = !element.isExpanded;
        }

        EditorGUI.indentLevel--;

        if (!element.isExpanded)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        // Check if an asset is referenced - if not we are done here
        if (!element.objectReferenceValue) return;

        // Otherwise get the SerializedObject for this asset
        SerializedObject elementSerializedObject = new(element.objectReferenceValue);

        // Find all properties in the referenced script that we want to display
        SerializedProperty sp = elementSerializedObject.GetIterator();
        sp.NextVisible(true);

        List<SerializedProperty> childProperties = new();

        while (sp.NextVisible(false))
        {
            childProperties.Add(sp.Copy());
        }

        // Similar to the OnInspectorGUI first load the current values into this SerializedObject
        elementSerializedObject.Update();

        // Adding some indentation just to show that the following fields are actually belonging to the referenced asset
        EditorGUI.indentLevel++;

        rect = EditorGUI.IndentedRect(rect);
        rect.y += EditorGUIUtility.singleLineHeight + 4;

        foreach (SerializedProperty child in childProperties)
        {
            float height = EditorGUI.GetPropertyHeight(child, true) + 4;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, height),
                child, true);
            elementSerializedObject.ApplyModifiedProperties();
            rect.y += height + 4;
        }

        EditorGUI.indentLevel--;

        // Write back the changed values and trigger the checks for logging dirty states and Undo/Redo
        elementSerializedObject.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        elementSerializedObject.Dispose();
    }

    #endregion
}