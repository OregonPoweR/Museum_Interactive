using UnityEngine;

namespace InteractionEffects
{
    public class SetClickable : InteractionEffect
    {
        [SerializeField] private Clickable targetClickable;
        [SerializeField] private bool isClickable;
        public override bool CheckSetup()
        {
            if (targetClickable == null)
            {
                Debug.LogError("Target Clickable not set on InteractionEffect! Please set it.");
            }

            return targetClickable != null;
        }

        public override void Trigger()
        {
            targetClickable.IsClickable = isClickable;
        }
    }
}
