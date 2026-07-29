using Editor;
using UnityEditor;
using Wrappers;

[CustomEditor(typeof(LockListWrapper), true)]
public class LockListEditor : ReorderableInteractionEffectLists
{
    protected override string[] GetListNames()
    {
        return new[]
        {
            "interactionEffectsOnConditionNotFulfilled",
            "interactionEffectsOnConditionPartiallyFulfilled",
            "interactionEffectsOnConditionFulfilled"
        };
    }
}