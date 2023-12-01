using System.Collections;
using UnityEngine;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Stat Initialisation")]
    public class InitialiseStatsAbilityScriptableObject : AbstractAbilityScriptableObject
    {
        public GameplayEffectScriptableObject[] InitialisationGE;

        public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
        {
            var spec = new InitialiseStatsAbility(this, owner);
            spec.Level = owner.Level;
            return spec;
        }

        public class InitialiseStatsAbility : AbstractAbilitySpec
        {
            public InitialiseStatsAbility(AbstractAbilityScriptableObject abilitySO, AbilitySystemCharacter owner) : base(abilitySO, owner)
            {
            }

            public override void CancelAbility()
            {
            }

            public override bool CheckGameplayTags()
            {
                return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                        && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags);
            }

            protected override IEnumerator ActivateAbility()
            {
                yield return null;
                InitialiseStatsAbilityScriptableObject abilitySO = this.Ability as InitialiseStatsAbilityScriptableObject;

                for (var i = 0; i < abilitySO.InitialisationGE.Length; i++)
                {
                    var effectSpec = this.Owner.MakeOutgoingSpec(abilitySO.InitialisationGE[i]);
                    this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);
                }

                yield break;
            }

            protected override void PreActivate()
            {
            }
        }
    }

}