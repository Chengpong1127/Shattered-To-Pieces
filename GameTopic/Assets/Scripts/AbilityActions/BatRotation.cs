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
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new BatRotationSpec(this, owner);
        return spec;
    }

    public class BatRotationSpec : EntityAbilitySpec
    {
        private Animator entityAnimator;

        public BatRotationSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            entityAnimator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
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
            entityAnimator.SetTrigger("Swing");
            yield return new WaitUntil(() => entityAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
            Debug.Log("Swing end");
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}