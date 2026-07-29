using Editor;
using UnityEditor;
using Wrappers;

[CustomEditor(typeof(ClickableListWrapper), true)]
public class ClickableListEditor : ReorderableInteractionEffectLists
{
    protected override string[] GetListNames()
    {
        return new[]
        {
            "interactionEffectsOnClicked"
        };
    }
}