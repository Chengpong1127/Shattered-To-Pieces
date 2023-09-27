using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "PropellerFly", menuName = "Ability/PropellerFly")]
public class PropellerFly : DisplayableAbilityScriptableObject {

    [SerializeField] float Power;
    
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new PropellerFlySpec(this, owner) {
            Power = Power
        };

        return spec;
    }

    public class PropellerFlySpec : RunnerAbilitySpec {
        public float Power;

        Animator animator;
        public PropellerFlySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
        }

        public override void CancelAbility()
        {
            EndAbility();
        }

        protected override IEnumerator ActivateAbility() {
            if(isActive) { animator.SetBool("Fly",true); }
            while (isActive) {
                var gameComponent = SelfEntity as GameComponent;
                //SelfEntity.BodyRigidbody.velocity = gameComponent.AssemblyTransform.TransformDirection(Vector3.up) * Power;
                SelfEntity.BodyRigidbody.AddForce(gameComponent.AssemblyTransform.TransformDirection(Vector3.up) * Power, ForceMode2D.Force);
                yield return null;
            }
            animator.SetBool("Fly", false);
        }
    }
}
