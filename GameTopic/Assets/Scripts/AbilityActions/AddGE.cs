using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LoaderPush;

[CreateAssetMenu(fileName = "AddGE", menuName = "Ability/AddGE")]
public class AddGE : DisplayableAbilityScriptableObject {
    public GameplayEffectScriptableObject gameEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new AddGESpec(this, owner) {
            gameEffect = gameEffect
        };
        return spec;
    }

    public class AddGESpec : RunnerAbilitySpec {
        public GameplayEffectScriptableObject gameEffect;
        public AddGESpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(SelfEntity, SelfEntity, gameEffect);
            yield return null;
        }

        protected override IEnumerator PreActivate() {
            yield return null;
        }
    }
}
