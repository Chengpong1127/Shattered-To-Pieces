using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WindAbility", menuName = "Ability/WindAbility")]
public class WindAbility : DisplayableAbilityScriptableObject
{

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new WindAbilitySpec(this, owner)
        {
        };
        return spec;
    }

    public class WindAbilitySpec : RunnerAbilitySpec
    {
        ICharacterCtrl Character;
        BaseCoreComponent body;
        Animator animator;

        bool Active;

        public WindAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            body = obj.body;
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            Active = false;
        }

        public override void CancelAbility()
        {
            animator.SetBool("Blow", false);
            Active = false;

            return;
        }

        public override bool CheckGameplayTags()
        {
            return true;
        }

        protected override IEnumerator ActivateAbility()
        {

            while (Active)
            {
                animator.SetBool("Blow", true);
                if (SelfEntity.transform.parent.localScale.x>0)
                {
                    Character.HorizontalMove(3f);
                }
                else
                {
                    Character.HorizontalMove(-3f);
                }
                yield return null;
            }


            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            Character = body.GetRoot() as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl"); 
            Active = true;

            yield return null;
        }
    }
}
