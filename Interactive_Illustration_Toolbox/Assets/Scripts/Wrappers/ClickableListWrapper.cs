using System.Collections.Generic;
using InteractionEffects;
using UnityEngine;

namespace Wrappers
{
    public class ClickableListWrapper : MonoBehaviour
    {
        public List<InteractionEffect> interactionEffectsOnClicked = new();
    }
}
