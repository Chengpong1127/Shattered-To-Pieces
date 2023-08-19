using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;

[CreateAssetMenu(fileName = "RotationTriggerAbility", menuName = "Ability/RotationTriggerAbility")]
public class RotationTriggerEntityAbility: RotationAbility{
    public GameplayEffectScriptableObject GameplayEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RotationTriggerAbilitySpec(this, owner);
        var rotateComponent = owner.GetComponentInParent<IRotatable>();
        if (rotateComponent == null)
        {
            Debug.LogError("RotationAbility requires a IRotatable component on the owner");
            return null;
        }
        spec.RotationTransform = rotateComponent.RotateBody;
        spec.RotateCenter = rotateComponent.RotateCenter;
        spec.RotationTime = DurationTime;
        spec.RotationValue = RotationValue;
        spec.GameplayEffect = GameplayEffect;
        spec.TriggerEntity = owner.GetComponentInParent<ITriggerEntity>() ?? throw new System.Exception("RotationTriggerEntityAbility requires a ITriggerEntity component on the owner");
        return spec;
    }
    protected class RotationTriggerAbilitySpec : RotationAbilitySpec
    {
        public ITriggerEntity TriggerEntity;
        public GameplayEffectScriptableObject GameplayEffect;
        public RotationTriggerAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner){

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

            throw new System.NotImplementedException();
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
        private void TriggerAction(Entity entity){
            var spec = Owner.MakeOutgoingSpec(GameplayEffect);
            entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(spec);
        }
    }
}