using AbilitySystem;
using AbilitySystem.Authoring;
using System;
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
        bool _IsAniamtorFly = false;
        bool SetAnimatorSwitch = false;


        bool IsAnimatorFly {
            get { return _IsAniamtorFly; }
            set {
                _IsAniamtorFly = value;
                if (SetAnimatorSwitch) { return; }
                SetAnimatorSwitch = true;
                SelfEntity.StartCoroutine(SetAnimatorValue());
            }
        }

        Animator animator;
        public PropellerFlySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
        }

        public override void CancelAbility()
        {
            IsAnimatorFly = false;
            EndAbility();
        }

        protected override IEnumerator ActivateAbility() {
            if(isActive) { IsAnimatorFly = true; }
            while (isActive) {
                if (EnergyManager.HasEnergy(EnergyCostPerSecond * Time.fixedDeltaTime * 2))
                    EnergyManager.CostEnergy(EnergyCostPerSecond * Time.fixedDeltaTime);
                else {
                    CancelAbility();
                    yield break;
                }
                var forceAddable = SelfEntity as IForceAddable;
                forceAddable.AddForce(SelfEntity.BodyTransform.TransformDirection(Vector3.up) * Power, ForceMode2D.Force);
                yield return new WaitForFixedUpdate();
            }
            IsAnimatorFly = false;
        }

        IEnumerator SetAnimatorValue() {
            yield return new WaitForFixedUpdate();

            animator.SetBool("Fly", _IsAniamtorFly);
            SetAnimatorSwitch = false;
            yield return null;
        }
    }
}
