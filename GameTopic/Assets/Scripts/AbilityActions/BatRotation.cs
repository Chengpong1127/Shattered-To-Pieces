using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using DG.Tweening;
[CreateAssetMenu(fileName = "BatRotation", menuName = "Ability/BatRotation")]
public class BatRotation : AbstractAbilityScriptableObject
{
    [SerializeField]
    public float Angle;
    [SerializeField]
    public float DurationTime;
    [SerializeField]
    public Ease EaseMode;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new BatRotationSpec(this, owner){
            Angle = Angle,
            DurationTime = DurationTime,
            EaseMode = EaseMode
        };

        return spec;
    }

    public class BatRotationSpec : EntityAbilitySpec
    {
        public Transform RotationTransform;
        public Transform RotateCenter;
        public float Angle;
        public float DurationTime;
        public Ease EaseMode;

        public BatRotationSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            var rotatable = SelfEntity as IRotatable ?? throw new System.ArgumentNullException("SelfEntity");
            RotationTransform = rotatable.RotateBody;
            RotateCenter = rotatable.RotateCenter;
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
            Vector3 originPosition = RotationTransform.localPosition;
            var rotation = RotationTransform.DOLocalRotate(new Vector3(0, 0, Angle) + RotationTransform.localPosition, DurationTime, RotateMode.FastBeyond360).SetEase(EaseMode);
            yield return rotation.WaitForCompletion();
            rotation = RotationTransform.DOLocalRotate(originPosition, DurationTime, RotateMode.FastBeyond360).SetEase(EaseMode);
            yield return rotation.WaitForCompletion();
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}