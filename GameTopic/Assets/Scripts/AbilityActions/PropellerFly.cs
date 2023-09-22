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
        bool Active;

        BaseCoreComponent Body;
        ICharacterCtrl Character;
        Animator animator;
        bool addConfirm = false;
        public PropellerFlySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;
            Active = false;
        }
        public override void CancelAbility() {
            Active = false;
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }
        protected override IEnumerator ActivateAbility() {
            if(Active) { animator.SetBool("Fly",true); }
            while (Active) {
                var gameComponent = SelfEntity as GameComponent;
                //SelfEntity.BodyRigidbody.velocity = gameComponent.AssemblyTransform.TransformDirection(Vector3.up) * Power;
                SelfEntity.BodyRigidbody.AddForce(gameComponent.AssemblyTransform.TransformDirection(Vector3.up) * Power, ForceMode2D.Force);
                yield return null;
            }
            animator.SetBool("Fly", false);
        }
        protected override IEnumerator PreActivate() {
            Character = Body.GetRoot() as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl");
            Active = Character != null;
            addConfirm = false;

            yield return null;
        }
    }
}
