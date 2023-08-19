using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;
using DG.Tweening;

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
        var spec = new MovePositionAbilitySpec(this, owner);
        var entity = owner.GetComponent<Entity>();
        spec.TargetTransform = entity.BodyTransform;
        spec.Local = Local;
        spec.Position = Position;
        spec.Duration = Duration;
        spec.EaseMode = EaseMode;
        return spec;
    }
    protected class MovePositionAbilitySpec : AbstractAbilitySpec
    {
        public Transform TargetTransform;
        public bool Local;
        public Vector2 Position;
        public float Duration;
        public Ease EaseMode;

        public MovePositionAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
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