using System;
using AbilitySystem.Authoring;
using GameplayTagNamespace.Authoring;

namespace AbilitySystem
{
    [Serializable]
    public struct ConditionalGameplayEffectContainer
    {
        public GameplayEffectScriptableObject GameplayEffect;
        public GameplayTagNamespace.Authoring.GameplayTag[] RequiredSourceTags;
    }

}
