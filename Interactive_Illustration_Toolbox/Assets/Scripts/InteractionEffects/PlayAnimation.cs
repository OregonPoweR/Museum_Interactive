using UnityEngine;

namespace InteractionEffects
{
    public class PlayAnimation : InteractionEffect
    {
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private string animationName;
    
        public override bool CheckSetup()
        {
            if (targetAnimator == null)
            {
                Debug.LogError("Target Animator not set on InteractionEffect! Please set it.");
            }

            return targetAnimator != null;
        }

        public override void Trigger()
        {
            targetAnimator.Play(animationName);
        }
    }
}
