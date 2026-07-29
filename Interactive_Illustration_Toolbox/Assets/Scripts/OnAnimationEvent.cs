using InteractionEffects;
using UnityEngine;
using Wrappers;

public class OnAnimationEvent : MonoBehaviour
{
    [SerializeField] private bool fireOnce = false;
    [SerializeField] private AnimationEventWrapper wrapper;

    private bool hasFired = false;
    
    public void OnAnimationEventFire()
    {
        if (fireOnce && hasFired) return;
        
        hasFired = true;
        
        foreach (InteractionEffect interactionEffect in wrapper.interactionEffectsOnEventFire)
        {
            interactionEffect.TriggerDelayed();
        }
    }

#if UNITY_EDITOR
    private void Reset()
    {
        if (wrapper == null)
        {
            wrapper = GetComponent<AnimationEventWrapper>();
        }
    }
#endif
}