using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using DG.Tweening;
[CreateAssetMenu(fileName = "RotationAbility", menuName = "Ability/RotationAbility")]
public class RotationAbility : AbstractAbilityScriptableObject
{
    [SerializeField]
    protected bool Local;
    [SerializeField]
    protected float RotationValue;
    [SerializeField]
    protected float DurationTime;
    [SerializeField]
    protected RotateMode RotateMode;
    [SerializeField]
    protected Ease EaseMode;


    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RotationAbilitySpec(this, owner)
        {
            RotationTime = DurationTime,
            RotationValue = RotationValue,
            Local = Local,
            RotateMode = RotateMode,
            EaseMode = EaseMode
        };

        return spec;

    }
    public class RotationAbilitySpec : EntityAbilitySpec
    {
        public Transform RotationTransform;
        public Transform RotateCenter;
        public float RotationTime;
        public float RotationValue;
        public bool Local;
        public RotateMode RotateMode;
        public Ease EaseMode;
        public RotationAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner){
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

            if (Local){
                RotationTransform.DOLocalRotate(new Vector3(0, 0, RotationValue), RotationTime, RotateMode)
                .SetEase(EaseMode);
            }
            else{
                RotationTransform.DORotate(new Vector3(0, 0, RotationValue), RotationTime, RotateMode)
                .SetEase(EaseMode);
            }
            yield return new WaitForSeconds(RotationTime);
            
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

