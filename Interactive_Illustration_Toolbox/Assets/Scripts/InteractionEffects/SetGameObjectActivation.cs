using UnityEngine;

namespace InteractionEffects
{
    public class SetGameObjectActivation : InteractionEffect
    {
        [SerializeField]
        private GameObject targetGameObject;

        [SerializeField]
        private bool active;

        public override bool CheckSetup()
        {
            if (targetGameObject == null)
            {
                LogError("CheckSetup: GameObject to toggle was not set. Please set it.");
            }
            
            return targetGameObject != null;
        }

        public override void Trigger()
        {
            if (!SetupPassed) return;
            targetGameObject.SetActive(active);
        }
    }
}