using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;

[CreateAssetMenu(fileName = "RotationTriggerAbility", menuName = "Ability/RotationTriggerAbility")]
public class GiveEffectAbility: RotationAbility{
    [SerializeField]
    protected TargetChoice targetChoice;
    [SerializeField]
    protected GameplayEffectScriptableObject[] GameplayEffects;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new GiveEffectAbilityAbilitySpec(this, owner)
        {
            GameplayEffects = GameplayEffects,
            SelfEntity = owner.GetComponent<Entity>() ?? throw new System.ArgumentNullException("owner"),
            targetChoice = targetChoice,
        };
        return spec;
    }
    protected class GiveEffectAbilityAbilitySpec : RotationAbilitySpec
    {
        public TargetChoice targetChoice;
        public Entity SelfEntity;
        public GameplayEffectScriptableObject[] GameplayEffects;
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
            


            switch (targetChoice)
            {
                case TargetChoice.Self:
                    GiveEffect(SelfEntity);
                    break;
                case TargetChoice.WholeDevice:
                    var baseCoreComponent = SelfEntity as BaseCoreComponent ?? throw new System.ArgumentNullException("SelfEntity");
                    foreach (var entity in baseCoreComponent.GetAllChildren())
                    {
                        GiveEffect(entity);
                    }
                    break;
                default:
                    break;
            }
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
        private void GiveEffect(Entity entity){
            foreach(var effect in GameplayEffects)
            {
                var spec = Owner.MakeOutgoingSpec(effect);
                entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(spec);
            }
        }
    }

    protected enum TargetChoice{
        Self,
        WholeDevice,
    }
}