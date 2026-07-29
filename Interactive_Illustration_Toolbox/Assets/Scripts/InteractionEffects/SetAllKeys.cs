using InteractionEffects;
using UnityEngine;

public class SetAllKeys : InteractionEffect
{
    [SerializeField] private Lock targetLock;
    [SerializeField] private bool setToActivated;

    public override bool CheckSetup()
    {
        if (targetLock == null)
        {
            Debug.LogError("Target Lock not set on InteractionEffect! Please set it.");
        }

        return targetLock != null;    
    }

    public override void Trigger()
    {
        targetLock.SetAllKeys(setToActivated);
    }
}
