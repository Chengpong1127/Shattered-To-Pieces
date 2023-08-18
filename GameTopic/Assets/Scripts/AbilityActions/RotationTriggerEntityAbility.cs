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
        spec.RotationTime = RotationTime;
        spec.RotationAngle = RotationAngle;
        spec.RotateClockwise = RotateClockwise;
        spec.RotateBack = RotateBack;
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
            TriggerEntity.OnTriggerEnterEvent += TriggerAction;
            var rotationSpeed = RotationAngle / RotationTime;
            var time = 0f;
            while (time < RotationTime)
            {
                time += Time.deltaTime;
                RotationTransform.RotateAround(RotateCenter.position, Vector3.forward, rotationSpeed * Time.deltaTime * (RotateClockwise ? 1 : -1));
                yield return null;
            }
            if (RotateBack)
            {
                while (time > 0)
                {
                    time -= Time.deltaTime;
                    RotationTransform.RotateAround(RotateCenter.position, Vector3.forward, rotationSpeed * Time.deltaTime * (RotateClockwise ? -1 : 1));
                    yield return null;
                }
            }
            TriggerEntity.OnTriggerEnterEvent -= TriggerAction;
            EndAbility();
            
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