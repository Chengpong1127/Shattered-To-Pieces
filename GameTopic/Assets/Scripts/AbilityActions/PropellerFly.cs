using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using UnityEngine;


[CreateAssetMenu(fileName = "PropellerFly", menuName = "Ability/PropellerFly")]
public class PropellerFly : DisplayableAbilityScriptableObject {

    [SerializeField] float Power;
    [SerializeField] float EnergyCostPerSecond;
    
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new PropellerFlySpec(this, owner) {
            Power = Power,
            EnergyCostPerSecond = EnergyCostPerSecond,
        };

        return spec;
    }

    public class PropellerFlySpec : RunnerAbilitySpec {
        public float Power;
        public float EnergyCostPerSecond;

        Animator animator;
        public PropellerFlySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
        }

        public override void CancelAbility()
        {
            animator.SetBool("Fly", false);
            EndAbility();
        }

        protected override IEnumerator ActivateAbility() {
            if(isActive) { animator.SetBool("Fly",true); }
            while (isActive) {
                if (EnergyManager.HasEnergy(EnergyCostPerSecond * Time.fixedDeltaTime))
                    EnergyManager.CostEnergy(EnergyCostPerSecond * Time.fixedDeltaTime);
                else {
                    CancelAbility();
                    yield break;
                }
                var gameComponent = SelfEntity as GameComponent;
                SelfEntity.BodyRigibodyAddForce_ClientRpc(gameComponent.AssemblyTransform.TransformDirection(Vector3.up) * Power, ForceMode2D.Force);
                yield return new WaitForFixedUpdate();
            }
            animator.SetBool("Fly", false);
        }
    }
}
