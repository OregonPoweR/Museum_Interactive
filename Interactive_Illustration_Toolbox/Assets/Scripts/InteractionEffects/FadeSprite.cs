using System.Collections;
using Boilerplate;
using UnityEngine;

namespace InteractionEffects
{
    public class FadeSprite : InteractionEffect
    {
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private float targetOpacity = 0f;
        [SerializeField] private float fadeDuration = 1f;

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
            StartCoroutine(FadeRoutine());
        }

        public override float GetAdditionalWaitTime()
        {
            return fadeDuration;
        }

        private IEnumerator FadeRoutine()
        {
            float time = 0f;
            float startOpacity = targetSpriteRenderer.color.a;

            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                float relativeTime = time / fadeDuration;

                targetSpriteRenderer.color = targetSpriteRenderer.color.With(a : Mathf.Lerp(startOpacity, targetOpacity, relativeTime));
                yield return null;
            }
        }
    }
}
