using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;
using DG.Tweening;

[CreateAssetMenu(fileName = "MovePositionAbility", menuName = "Ability/MovePositionAbility")]
public class MovePositionAbility : AbstractAbilityScriptableObject
{
    /// <summary>
    /// If true, the position is set relative to the owner's position, otherwise it is set relative to the world.
    /// </summary>
    [SerializeField]
    protected bool SetLocal;
    /// <summary>
    /// The position to set.
    /// </summary>
    [SerializeField]
    protected Vector2 Position;
    [SerializeField]
    protected float Duration;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new MovePositionAbilitySpec(this, owner);
        var entity = owner.GetComponent<Entity>();
        spec.TargetTransform = entity.BodyTransform;
        spec.SetLocalPosition = SetLocal;
        spec.Position = Position;
        spec.Duration = Duration;
        return spec;
    }
    protected class MovePositionAbilitySpec : AbstractAbilitySpec
    {
        public Transform TargetTransform;
        public bool SetLocalPosition;
        public Vector2 Position;
        public float Duration;

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
            if (SetLocalPosition)
            {
                TargetTransform.DOLocalMove(Position, Duration);
            }
            else
            {
                TargetTransform.DOMove(Position, Duration);
            }
            yield return new WaitForSeconds(Duration);
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}