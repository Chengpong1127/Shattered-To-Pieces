using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;
using DG.Tweening;
using UnityEditor.UI;

[CreateAssetMenu(fileName = "MoveAbility", menuName = "Ability/MoveAbility")]
public class MoveAbility : AbstractAbilityScriptableObject
{
    /// <summary>
    /// If true, the position is set relative to the owner's position, otherwise it is set relative to the world.
    /// </summary>
    [SerializeField]
    protected bool Local;
    /// <summary>
    /// The position to set.
    /// </summary>
    [SerializeField]
    protected Vector2 Position;
    [SerializeField]
    protected float Duration;
    [SerializeField]
    protected Ease EaseMode;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new MovePositionAbilitySpec(this, owner)
        {
            Local = Local,
            Position = Position,
            Duration = Duration,
            EaseMode = EaseMode
        };
        return spec;
    }
    protected class MovePositionAbilitySpec : EntityAbilitySpec
    {
        public Transform TargetTransform;
        public bool Local;
        public Vector2 Position;
        public float Duration;
        public Ease EaseMode;

        public MovePositionAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            TargetTransform = SelfEntity.BodyTransform;
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
            if (Local)
            {
                TargetTransform.DOLocalMove(Position, Duration)
                    .SetEase(EaseMode);
            }
            else
            {
                TargetTransform.DOMove(Position, Duration)
                    .SetEase(EaseMode);
            }
            yield return new WaitForSeconds(Duration);
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}