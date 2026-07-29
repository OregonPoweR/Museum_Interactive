using System;
using UnityEngine;

namespace InteractionEffects
{
    /// <summary>
    /// Base class for things that should happen when an interaction has happened.
    /// </summary>
    [Serializable]
    public abstract class InteractionEffect : MonoBehaviour
    {
        /// <summary>
        /// True if the set up of the interaction effect is fine.
        /// </summary>
        public bool SetupPassed { get; private set; }

        [Tooltip("The delay before the effect is triggered in seconds.")]
        [Min(0)] public float delay = 0f;

        private void Awake() => SetupPassed = CheckSetup();

        /// <summary>
        /// Returns true if the setup is correctly setup.
        /// Overriding members should follow this pattern.
        /// </summary>
        public abstract bool CheckSetup();
    
        public abstract void Trigger();

        /// <summary>
        /// Returns any additional time the component might have to stay alive for, such as when running a coroutine to lerp an effect over time.
        /// </summary>
        public virtual float GetAdditionalWaitTime()
        {
            return 0f;
        }

        public void TriggerDelayed()
        {
            Invoke(nameof(Trigger), delay);
        }

        protected void LogError(string msg) => Debug.LogError($"[{GetType().Name}] {msg}", this);
    }
}