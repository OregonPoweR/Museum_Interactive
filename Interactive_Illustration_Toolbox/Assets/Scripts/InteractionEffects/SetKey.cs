using InteractionEffects;
using UnityEngine;

public class SetKey : InteractionEffect
{
    [SerializeField] private Key targetKey;
    [SerializeField] private bool isActivated;

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
        targetKey.ChangeStateTo(isActivated);
    }
}
