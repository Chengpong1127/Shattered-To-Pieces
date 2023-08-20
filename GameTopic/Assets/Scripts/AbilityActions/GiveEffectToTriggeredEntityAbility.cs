using System.Threading;
using AbilitySystem.Authoring;
using AbilitySystem;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "GiveEffectToTriggeredEntityAbility", menuName = "Ability/GiveEffectToTriggeredEntityAbility")]
public class GiveEffectToTriggeredEntityAbility : AbstractAbilityScriptableObject
{
    /// <summary>
    /// The number of entities that can be given effect by this ability.
    /// </summary>
    [SerializeField]
    protected int EntityTriggerCount;
    /// <summary>
    /// The effect duration.
    /// </summary>
    [SerializeField]
    protected float Duration;
    [SerializeField]
    protected GameplayEffectScriptableObject[] GameplayEffects;


    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new GiveEffectToTriggeredEntityAbilityAbilitySpec(this, owner)
        {
            EntityTriggerCount = EntityTriggerCount,
            Duration = Duration,
            GameplayEffects = GameplayEffects,
        };
        return spec;
    }

    public class GiveEffectToTriggeredEntityAbilityAbilitySpec : EntityAbilitySpec
    {
        public int EntityTriggerCount;
        public float Duration;
        public GameplayEffectScriptableObject[] GameplayEffects;
        private IEntityTriggerable EntityTriggerable;
        private int count;
        private float duration;
        public GiveEffectToTriggeredEntityAbilityAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            EntityTriggerable = SelfEntity as IEntityTriggerable ?? throw new System.Exception("SelfEntity must implement IEntityTriggerable");
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
            EntityTriggerable.OnTriggerEntity += GiveEffect;
            while (count < EntityTriggerCount && duration >= 0)
            {
                duration -= Time.deltaTime;
                yield return null;
            }
            EntityTriggerable.OnTriggerEntity -= GiveEffect;
            EndAbility();
        }

        protected override IEnumerator PreActivate()
        {
            count = 0;
            duration = Duration;
            yield return null;
        }
        private void GiveEffect(Entity entity)
        {
            Debug.Log("GiveEffect to " + entity.name + "");
            foreach (var effect in GameplayEffects)
            {
                this.TriggerEvent(EventName.GameEffectManagerEvents.RequestGiveGameEffect, SelfEntity, entity, effect);
            }
            count++;
        }
    }
}