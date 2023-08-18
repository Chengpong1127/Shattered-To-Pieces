using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
[CreateAssetMenu(fileName = "RigibodyControlAbility", menuName = "Ability/RigibodyControlAbility")]
public class RigibodyControlAbility : AbstractAbilityScriptableObject
{
    [SerializeField]
    private RigidbodyType2D SetRigidbodyType;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RigibodyControlAbilitySpec(this, owner);
        var baseCoreComponent = owner.GetComponent<BaseCoreComponent>();
        spec.Rigidbody2D = baseCoreComponent.BodyRigidbody;
        spec.SetRigidbodyType = SetRigidbodyType;
        return spec;
    }
    protected class RigibodyControlAbilitySpec : AbstractAbilitySpec
    {
        public Rigidbody2D Rigidbody2D;
        public RigidbodyType2D SetRigidbodyType;
        public RigibodyControlAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
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
            Rigidbody2D.bodyType = SetRigidbodyType;
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}
