using System;
using InteractionEffects;
using NaughtyAttributes;
using UnityEngine;
using Wrappers;

[RequireComponent(typeof(KeyListWrapper))]
public class Key : MonoBehaviour
{
    [SerializeField] private KeyListWrapper interactionEffectsList;
    public event Action<Key> OnKeyActivatedEvent;

    public bool IsRequired = false;
    [SerializeField] private bool onlyTriggerEffectsOnDifferentState = true;
    [SerializeField] private bool isActivated = false;
        
    public bool IsActivated
    {
        get => isActivated;
        private set => isActivated = value;
    }

    public void Reset()
    {
        if (interactionEffectsList == null)
        {
            interactionEffectsList = GetComponent<KeyListWrapper>();
        }
    }

    public void ChangeStateTo(bool nowActivated)
    {
        if ((isActivated != nowActivated && onlyTriggerEffectsOnDifferentState) || !onlyTriggerEffectsOnDifferentState)
        {
            if (nowActivated)
            {
                foreach (InteractionEffect effect in interactionEffectsList.interactionEffectsOnTurnedOn)
                {
                    effect.Trigger();
                }
            }
            else
            {
                foreach (InteractionEffect effect in interactionEffectsList.interactionEffectsOnTurnedOff)
                {
                    effect.Trigger();
                }
            }
        }
        IsActivated = nowActivated;
            
        OnKeyActivatedEvent?.Invoke(this);
    }
    
    [Button]
    public void ToggleState()
    {
        ChangeStateTo(!IsActivated);
    }
}
