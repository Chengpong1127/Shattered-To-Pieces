using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using DG.Tweening;

[CreateAssetMenu(fileName = "RotatorAbility", menuName = "Ability/RotatorAbility")]
public class RotatorAbility : AbstractAbilityScriptableObject {

    [SerializeField] protected float RotationValue;
    [SerializeField] protected float DurationTime;
    [SerializeField] protected RotateMode RotateMode;
    [SerializeField] protected Ease EaseMode;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new RotatorAbilitySpec(this, owner) {
            angle = RotationValue,
            duration = DurationTime,
            rotateMode = RotateMode,
            easeMode = EaseMode
        };

        spec.SetValues();
        return spec;
    }

    public class RotatorAbilitySpec : RunnerAbilitySpec {
        public float angle;
        public float duration;
        public Transform RotationTransform;
        public Transform RotateCenter;
        public RotateMode rotateMode;
        public Ease easeMode;

        Sequence abilitySequence;
        Vector3 angleVec;

        public RotatorAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            var rotatable = SelfEntity as IRotatable ?? throw new System.ArgumentNullException("SelfEntity");
            RotationTransform = rotatable.RotateBody;
            RotateCenter = rotatable.RotateCenter;
            abilitySequence = DOTween.Sequence();
            abilitySequence.SetLoops(-1,LoopType.Yoyo);
            
        }

        public override void CancelAbility() {
            abilitySequence.Restart();
            abilitySequence.Pause();
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }

        protected override IEnumerator ActivateAbility() {
            abilitySequence.Play();
            yield return null;
        }

        protected override IEnumerator PreActivate() {
            yield return null;
        }

        public void SetValues() {
            angleVec.z = angle;
            abilitySequence.Append(
                RotationTransform.DOLocalRotate(RotationTransform.localPosition + angleVec, duration, rotateMode)
                    .SetEase(easeMode));
            abilitySequence.Pause();
        }
    }
}
