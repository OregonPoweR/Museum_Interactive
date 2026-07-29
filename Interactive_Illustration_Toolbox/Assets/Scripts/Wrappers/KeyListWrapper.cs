using System.Collections.Generic;
using InteractionEffects;
using UnityEngine;

namespace Wrappers
{
    public class KeyListWrapper : MonoBehaviour
    {
        public List<InteractionEffect> interactionEffectsOnTurnedOn = new();
        public List<InteractionEffect> interactionEffectsOnTurnedOff = new();
    }
}