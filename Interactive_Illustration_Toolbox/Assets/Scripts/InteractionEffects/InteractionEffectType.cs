using System;

namespace InteractionEffects
{
    public enum InteractionEffectType : byte
    {
        FadeSprite = 0,
        PlayAnimation = 1,
        SetAnimatorTrigger = 2,
        LoadScene = 3,
        SetGameObjectActivation = 4,
        PlaySound = 5,
        SetSpriteColor = 6,
        SetKey = 7,
        ToggleKey = 8,
        SetAllKeys = 9,
        SetClickable
    }

    public static class InteractionEffectTypeExtension
    {
        public static Type GetInteractionEffectType(this InteractionEffectType interactionEffectType)
        {
            switch (interactionEffectType)
            {
                case InteractionEffectType.FadeSprite:
                    return typeof(FadeSprite);
                case InteractionEffectType.PlayAnimation:
                    return typeof(PlayAnimation);
                case InteractionEffectType.SetAnimatorTrigger:
                    return typeof(SetAnimatorTrigger);
                case InteractionEffectType.LoadScene:
                    return typeof(LoadScene);
                case InteractionEffectType.SetGameObjectActivation:
                    return typeof(SetGameObjectActivation);
                case InteractionEffectType.PlaySound:
                    return typeof(PlaySound);
                case InteractionEffectType.SetSpriteColor:
                    return typeof(SetSpriteColor);
                case InteractionEffectType.SetKey:
                    return typeof(SetKey);
                case InteractionEffectType.ToggleKey:
                    return typeof(ToggleKey);
                case InteractionEffectType.SetAllKeys:
                    return typeof(SetAllKeys);
                case InteractionEffectType.SetClickable:
                    return typeof(SetClickable);
                default:
                    throw new ArgumentOutOfRangeException(nameof(interactionEffectType), interactionEffectType, null);
            }
        }

        public static string GetMenuParentString(this InteractionEffectType interactionEffectType)
        {
            switch (interactionEffectType)
            {
                case InteractionEffectType.FadeSprite:
                    return "Sprites/";
                case InteractionEffectType.SetSpriteColor:
                    return "Sprites/";
                case InteractionEffectType.PlayAnimation:
                    return "Animation/";
                case InteractionEffectType.SetAnimatorTrigger:
                    return "Animation/";
                case InteractionEffectType.SetKey:
                    return "Key-Lock System/Key/";
                case InteractionEffectType.ToggleKey:
                    return "Key-Lock System/Key/";
                case InteractionEffectType.SetAllKeys:
                    return "Key-Lock System/Lock/";
                default:
                    return "";
            }
        }
    }
}