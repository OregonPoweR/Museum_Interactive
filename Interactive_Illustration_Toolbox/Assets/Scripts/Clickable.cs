using System;
using System.Collections;
using System.Xml;
using InteractionEffects;
using Unity.VisualScripting;
using UnityEngine;
using Wrappers;

[RequireComponent(typeof(ClickableListWrapper))]
public class Clickable : MonoBehaviour
{
    public int Priority = 0;
    public bool IsClickable = true;
    [SerializeField] private bool clickableOnce = false;
    [SerializeField] private bool blockDuringInteractionEffects = true;
    [SerializeField] private ClickableListWrapper wrapper;


    /// <summary>
    /// Fires InteractionEffects defined in the Wrapper. Called from the Player script when clicked.
    /// </summary>
    public void OnClick()
    {
        if (!IsClickable) return;

        if (clickableOnce) IsClickable = false;

        float waitTime = 0f;
        
        foreach (InteractionEffect interactionEffect in wrapper.interactionEffectsOnClicked)
        {
            interactionEffect.TriggerDelayed();
            float interactionEffectDuration = interactionEffect.delay + interactionEffect.GetAdditionalWaitTime();

            if (interactionEffectDuration > waitTime) waitTime = interactionEffectDuration;
        }

        if (!clickableOnce && blockDuringInteractionEffects && waitTime > 0f)
        {
            IsClickable = false;

            StartCoroutine(SetClickableAfterDelay(waitTime));
        }
    }

    private IEnumerator SetClickableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        IsClickable = true;
    }
    
#if UNITY_EDITOR
    private void Reset()
    {
        if (wrapper == null)
        {
            wrapper = GetComponent<ClickableListWrapper>();
        }
    }
#endif
}
