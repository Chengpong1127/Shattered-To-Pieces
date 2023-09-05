using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WindAbility", menuName = "Ability/WindAbility")]
public class WindAbility : AbstractAbilityScriptableObject
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
        public SpriteRenderer fan;
        public ICharacterCtrl Character;
        public BaseCoreComponent body;
        bool Active;
        public WindAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            
            fan=SelfEntity.gameObject.GetComponent<SpriteRenderer>();
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            body = obj.body;
            Active = false;
        }

        public override void CancelAbility()
        {
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
                if (fan.flipX)
                {
                    Character.HorizontalMove(-3f);
                }
                else
                {
                    Character.HorizontalMove(3f);
                }
                yield return null;
            }


            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            Character = body.Root as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl"); 
            Active = true;
            yield return null;
        }
    }
}
