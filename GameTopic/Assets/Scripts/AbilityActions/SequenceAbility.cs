using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;
[CreateAssetMenu(fileName = "SequenceAbility", menuName = "Ability/SequenceAbility")]
public class SequenceAbility : AbstractAbilityScriptableObject
{
    [SerializeField] 
    protected AbstractAbilityScriptableObject[] Abilities;
    [SerializeField]
    protected bool TerminateOnCancel;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new SequenceAbilitySpec(this, owner)
        {
            Abilities = Abilities,
            TerminateOnCancel = TerminateOnCancel
        };
        return spec;
    }
    protected class SequenceAbilitySpec : AbstractAbilitySpec
    {
        public AbstractAbilityScriptableObject[] Abilities;
        public bool TerminateOnCancel;
        private AbstractAbilitySpec[] Specs;
        private int CurrentIndex = 0;
        public SequenceAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {

        }
        public override void CancelAbility()
        {
            if (TerminateOnCancel)
            {
                Specs[CurrentIndex].CancelAbility();
                EndAbility();
            }
        }

        public override bool CheckGameplayTags()
        {
            return true;
        }

        protected override IEnumerator ActivateAbility()
        {
            for (CurrentIndex = 0; CurrentIndex < Specs.Length; CurrentIndex++)
            {
                yield return Specs[CurrentIndex].TryActivateAbility();
            }
            EndAbility();
        }

        protected override IEnumerator PreActivate()
        {
            Specs = new AbstractAbilitySpec[Abilities.Length];
            for (int i = 0; i < Abilities.Length; i++)
            {
                Specs[i] = Abilities[i].CreateSpec(Owner);
                Owner.GrantAbility(Specs[i]);
            }
            yield return null;
        }
    }
}