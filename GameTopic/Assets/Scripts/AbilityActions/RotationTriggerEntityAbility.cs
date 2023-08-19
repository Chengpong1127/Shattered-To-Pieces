using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;

[CreateAssetMenu(fileName = "RotationTriggerAbility", menuName = "Ability/RotationTriggerAbility")]
public class GiveEffectAbility: RotationAbility{
    [SerializeField]
    protected GameplayEffectScriptableObject GameplayEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new GiveEffectAbilityAbilitySpec(this, owner)
        {
            GameplayEffect = GameplayEffect,
            TriggerEntity = owner.GetComponentInParent<ITriggerEntity>() ?? throw new System.Exception("RotationTriggerEntityAbility requires a ITriggerEntity component on the owner")
        };
        return spec;
    }
    protected class GiveEffectAbilityAbilitySpec : RotationAbilitySpec
    {
        public ITriggerEntity TriggerEntity;
        public GameplayEffectScriptableObject GameplayEffect;
        public GiveEffectAbilityAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner){

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

    protected enum TargetChoice{
        Self,
        TriggerEntity,
    }
}