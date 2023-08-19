using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;

[CreateAssetMenu(fileName = "WaitAbility", menuName = "Ability/WaitAbility")]
public class WaitAbility: AbstractAbilityScriptableObject{
    [SerializeField] 
    protected float WaitSeconds;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new WaitAbilitySpec(this, owner)
        {
            WaitSeconds = WaitSeconds
        };
        return spec;
    }
    protected class WaitAbilitySpec : AbstractAbilitySpec
    {
        public float WaitSeconds;
        public WaitAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {

        }
        public override void CancelAbility()
        {
            return;
        }

        public override bool CheckGameplayTags()
        {
            return true;
        }

        protected override IEnumerator ActivateAbility()
        {
            yield return new WaitForSeconds(WaitSeconds);
            EndAbility();
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}