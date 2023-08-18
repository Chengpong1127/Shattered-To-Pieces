using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
[CreateAssetMenu(fileName = "RotationAbility", menuName = "Ability/RotationAbility")]
public class RotationAbility : AbstractAbilityScriptableObject
{
    [SerializeField]
    protected float RotationTime;
    [SerializeField]
    protected float RotationAngle;
    [SerializeField]
    protected bool RotateClockwise;
    [SerializeField]
    protected bool RotateBack;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RotationAbilitySpec(this, owner);
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
        return spec;

    }
    protected class RotationAbilitySpec : AbstractAbilitySpec
    {
        public Transform RotationTransform;
        public Transform RotateCenter;
        public float RotationTime;
        public float RotationAngle;
        public bool RotateClockwise;
        public bool RotateBack;
        public RotationAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner){

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
            EndAbility();
            
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

