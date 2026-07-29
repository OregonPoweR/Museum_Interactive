using UnityEngine;

namespace InteractionEffects
{
    public class SetAnimatorTrigger : InteractionEffect
    {
        [Tooltip("The animator of which the trigger will be set.")]
        [SerializeField]
        private Animator targetAnimator;

        [Tooltip("The name of the trigger that will be set.\n\n" +
                 "Case sensitive.")]
        [SerializeField]
        private string triggerName;

        public override bool CheckSetup()
        {
            if (targetAnimator == null)
            {
                Debug.LogError("CheckSetup: Target animator was not set. Please set it.");
            }

            if (string.IsNullOrWhiteSpace(triggerName))
            {
                Debug.LogError("CheckSetup: Trigger name was not set. Please set it.");
            }
            
            return targetAnimator != null && !string.IsNullOrWhiteSpace(triggerName);
        }

        /// <summary>
        /// Sets the targeted animator's trigger. 
        /// </summary>
        public override void Trigger()
        {
            if (!SetupPassed) return;
            targetAnimator.SetTrigger(triggerName);
        }
    }
}