using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RotatorAbility", menuName = "Ability/RotatorAbility")]
public class RotatorAbility : AbstractAbilityScriptableObject {

    [SerializeField] float Speed;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new RotatorAbilitySpec(this, owner) {
            Speed = Speed
        };
        return spec;
    }

    public class RotatorAbilitySpec : RunnerAbilitySpec {

        public float Speed;
        Animator animator;
        bool Active;
        public RotatorAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
        }

        public override void CancelAbility() {
            Active = false;
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            animator.SetBool("Rotate", true);
            yield return new WaitUntil(() =>  !Active );
            animator.SetBool("Rotate", false);
            yield return null;
        }

        protected override IEnumerator PreActivate() {
            Active = true;
            animator.SetFloat("Speed", Speed);
            yield return null;
        }
    }
}
