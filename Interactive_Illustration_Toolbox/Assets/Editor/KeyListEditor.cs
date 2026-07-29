using Editor;
using UnityEditor;
using Wrappers;

[CustomEditor(typeof(KeyListWrapper), true)]
public class KeyListEditor : ReorderableInteractionEffectLists
{
    protected override string[] GetListNames()
    {
        return new[]
        {
            "interactionEffectsOnTurnedOn",
            "interactionEffectsOnTurnedOff"
        };
    }
}