using InteractionEffects;
using UnityEngine;

public class ToggleKey : InteractionEffect
{
    [SerializeField] private Key targetKey;

    public override bool CheckSetup()
    {
        if (targetKey == null)
        {
            Debug.LogError("Target Key not set on InteractionEffect! Please set it.");
        }

        return targetKey != null;    
    }

    public override void Trigger()
    {
        targetKey.ChangeStateTo(!targetKey.IsActivated);
    }
}
