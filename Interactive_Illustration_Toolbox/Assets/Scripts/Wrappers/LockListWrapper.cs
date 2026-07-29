using System.Collections.Generic;
using InteractionEffects;
using UnityEngine;

namespace Wrappers
{
    public class LockListWrapper : MonoBehaviour
    {
        public List<InteractionEffect> interactionEffectsOnConditionNotFulfilled = new();
        public List<InteractionEffect> interactionEffectsOnConditionPartiallyFulfilled = new();
        public List<InteractionEffect> interactionEffectsOnConditionFulfilled = new();
    }
}
