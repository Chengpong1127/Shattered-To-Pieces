using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ChickenAttack", menuName = "Ability/ChickenAttack")]
public class ChickenAttack : AbstractAbilityScriptableObject {
    public GameObject SummonPrefab;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new ChickenAttackSpec(this, owner) {
            SummonPrefab = SummonPrefab
        };
        return spec;
    }

    public class ChickenAttackSpec : RunnerAbilitySpec {
        public GameObject SummonPrefab;

        Animator entityAnimator;
        ISummonable summonMachine;
        public ChickenAttackSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            summonMachine = (SelfEntity as ISummonable) ?? throw new System.ArgumentNullException("The entity should have ISummonable interface.");
        }

        public override void CancelAbility() {
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            entityAnimator.SetTrigger("Biu");
            summonMachine.InitSummonObject(SummonPrefab);
            yield return null;
        }

        protected override IEnumerator PreActivate() {
            yield return null;
        }

        private void TriggerAction(Entity other) {
        }
    }


}
