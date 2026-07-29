using System;
using System.Collections.Generic;
using InteractionEffects;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Wrappers;


[RequireComponent(typeof(LockListWrapper))]
public class Lock : MonoBehaviour
{
    [SerializeField] private LockListWrapper interactionEffectsList;

    [SerializeField] private List<Key> keysToCheck = new List<Key>();
    [SerializeField] private FulfillmentConditionType fulfillmentCondition = FulfillmentConditionType.All;

    [EnableIf("usingNumber")] [SerializeField]
    private int numberToCheck = 1;

    [SerializeField] private bool onlyTriggerEffectsOnDifferentConditionFulfillment = true;
    [SerializeField] private bool checkForRequiredEmitters = false;

    [ShowNonSerializedField]
    private ConditionFulfillmentType currentConditionFulfillment = ConditionFulfillmentType.NotFulfilled;

    //Tracks if number should be shown in editor.
    private bool usingNumber = false;

    private void Awake()
    {
        foreach (Key key in keysToCheck)
        {
            key.OnKeyActivatedEvent += OnKeyActivated;
        }
    }

    private void OnDestroy()
    {
        foreach (Key key in keysToCheck)
        {
            key.OnKeyActivatedEvent -= OnKeyActivated;
        }
    }
    
    [Button]
    public void ResetAllKeys()
    {
        SetAllKeys(false);
    }
    
    public void SetAllKeys(bool activated)
    {
        foreach (Key key in keysToCheck)
        {
            key.ChangeStateTo(activated);
        }
    }

    /// <summary>
    /// When a signal (Event fired by the SignalEmitter) is received we check all conditions and act accordingly.
    /// </summary>
    /// <param name="key"></param>
    private void OnKeyActivated(Key key)
    {
        foreach (Key keyToCheck in keysToCheck)
        {
            if (key == keyToCheck)
            {
                Debug.Log("Key change received, state is " + keyToCheck.IsActivated);
            }
        }

        CheckConditions();
    }

    /// <summary>
    /// Checks Conditions and triggers Interaction Effects.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void CheckConditions()
    {
        int amountFulfilled = 0;
        bool anyRequiredNotFulfilled = false;

        foreach (Key key in keysToCheck)
        {
            if (key.IsActivated)
            {
                amountFulfilled++;
            }
            else if (key.IsRequired)
            {
                anyRequiredNotFulfilled = true;
            }
        }

        ConditionFulfillmentType newConditionFulfillment =
            GetConditionCheckResult(amountFulfilled, anyRequiredNotFulfilled);

        if ((onlyTriggerEffectsOnDifferentConditionFulfillment &&
             newConditionFulfillment != currentConditionFulfillment) ||
            !onlyTriggerEffectsOnDifferentConditionFulfillment)
        {
            switch (newConditionFulfillment)
            {
                case ConditionFulfillmentType.NotFulfilled:
                    foreach (InteractionEffect effect in interactionEffectsList
                                 .interactionEffectsOnConditionNotFulfilled)
                    {
                        effect.TriggerDelayed();
                    }

                    break;
                case ConditionFulfillmentType.PartiallyFulfilled:
                    foreach (InteractionEffect effect in interactionEffectsList
                                 .interactionEffectsOnConditionPartiallyFulfilled)
                    {
                        effect.TriggerDelayed();
                    }

                    break;
                case ConditionFulfillmentType.AllFulfilled:
                    foreach (InteractionEffect effect in interactionEffectsList
                                 .interactionEffectsOnConditionFulfilled)
                    {
                        effect.TriggerDelayed();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.Log("Lock new state: " + newConditionFulfillment);
        }

        currentConditionFulfillment = newConditionFulfillment;
    }

    private ConditionFulfillmentType GetConditionCheckResult(int amountFulfilled, bool anyRequiredNotFulfilled)
    {
        ConditionFulfillmentType result = ConditionFulfillmentType.NotFulfilled;

        //Check condition
        switch (fulfillmentCondition)
        {
            case FulfillmentConditionType.All:
                if (amountFulfilled == keysToCheck.Count)
                {
                    result = ConditionFulfillmentType.AllFulfilled;
                }
                else if (amountFulfilled != 0)
                {
                    result = ConditionFulfillmentType.PartiallyFulfilled;
                }

                break;
            case FulfillmentConditionType.Any:
                if (amountFulfilled != 0)
                {
                    if (checkForRequiredEmitters && anyRequiredNotFulfilled)
                    {
                        result = ConditionFulfillmentType.PartiallyFulfilled;
                    }
                    else
                    {
                        result = ConditionFulfillmentType.AllFulfilled;
                    }
                }

                break;
            case FulfillmentConditionType.Number:
                if (amountFulfilled != 0)
                {
                    if (amountFulfilled >= numberToCheck)
                    {
                        if (!checkForRequiredEmitters || (checkForRequiredEmitters && !anyRequiredNotFulfilled))
                        {
                            result = ConditionFulfillmentType.AllFulfilled;
                        }
                        else
                        {
                            result = ConditionFulfillmentType.PartiallyFulfilled;
                        }
                    }
                    else
                    {
                        result = ConditionFulfillmentType.PartiallyFulfilled;
                    }
                }

                break;
        }

        return result;
    }

    public enum ConditionFulfillmentType : byte
    {
        NotFulfilled = 0,
        PartiallyFulfilled = 1,
        AllFulfilled = 2
    }

    public enum FulfillmentConditionType : byte
    {
        All = 0,
        Any = 1,
        Number = 2
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (Key key in keysToCheck)
        {
            if (key == null) continue;

            Vector3 startPos = transform.position;
            Vector3 endPos = key.transform.position;
            float halfPosition = (startPos.y - endPos.y) * .5f;
            Vector3 offset = Vector3.up * halfPosition;

            Color color = key.IsActivated ? Color.green : Color.red;
            Handles.DrawBezier(startPos, endPos, startPos - offset, endPos + offset, color,
                EditorGUIUtility.whiteTexture, 3f);
            Gizmos.color = color;
            Gizmos.DrawSphere(key.transform.position, .075f);
        }

        switch (currentConditionFulfillment)
        {
            case ConditionFulfillmentType.NotFulfilled:
                Gizmos.color = Color.red;
                break;
            case ConditionFulfillmentType.PartiallyFulfilled:
                Gizmos.color = Color.yellow;
                break;
            case ConditionFulfillmentType.AllFulfilled:
                Gizmos.color = Color.green;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Gizmos.DrawSphere(transform.position, .1f);
    }

    public void Reset()
    {
        if (interactionEffectsList == null)
        {
            interactionEffectsList = GetComponent<LockListWrapper>();
        }
    }

    private void OnValidate()
    {
        usingNumber = fulfillmentCondition == FulfillmentConditionType.Number;
        CheckConditions();
    }
#endif
}