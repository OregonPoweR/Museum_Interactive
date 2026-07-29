using System.Collections;
using Boilerplate;
using UnityEngine;

namespace InteractionEffects
{
    public class SetSpriteColor : InteractionEffect
    {
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Color newColor = Color.white;
        [SerializeField] private float transitionTime = 0f;
        [SerializeField] private bool includeTransparency = false;

        private Coroutine lerpRoutine = null;

        public override bool CheckSetup()
        {
            if (targetSpriteRenderer == null)
            {
                Debug.LogError("Target SpriteRenderer not set on InteractionEffect! Please set it.");
            }

            return targetSpriteRenderer != null;    
        }

        public override void Trigger()
        {
            if (transitionTime <= 0f)
            {
                targetSpriteRenderer.color = newColor;
            }
            else
            {
                if (lerpRoutine != null)
                {
                    StopCoroutine(lerpRoutine);
                }
            
                lerpRoutine = StartCoroutine(LerpColor());
            }
        }

        public override float GetAdditionalWaitTime()
        {
            return transitionTime;
        }

        private IEnumerator LerpColor()
        {
            float timePassed = 0f;
            Color startColor = targetSpriteRenderer.color;

            while (timePassed <= transitionTime)
            {
                timePassed += Time.deltaTime;
                targetSpriteRenderer.color = includeTransparency ? Color.Lerp(startColor, newColor, timePassed / transitionTime).With(a:targetSpriteRenderer.color.a) : Color.Lerp(startColor, newColor, timePassed / transitionTime);
                yield return null;
            }
        }
    }
}
