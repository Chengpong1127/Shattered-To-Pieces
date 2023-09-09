using System;
using GameplayTagNamespace.Authoring;

namespace AbilitySystem
{
    [Serializable]
    public struct GameplayTagRequireIgnoreContainer
    {
        /// <summary>
        /// All of these tags must be present
        /// </summary>
        public GameplayTagNamespace.Authoring.GameplayTag[] RequireTags;

        /// <summary>
        /// None of these tags can be present
        /// </summary>
        public GameplayTagNamespace.Authoring.GameplayTag[] IgnoreTags;
    }

}
