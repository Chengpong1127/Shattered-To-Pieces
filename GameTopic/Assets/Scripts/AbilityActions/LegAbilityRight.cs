using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttributeSystem.Components;
using AttributeSystem.Authoring;
[CreateAssetMenu(fileName = "LegAbilityRight", menuName = "Ability/LegAbilityRight")]
public class LegAbilityRight : DisplayableAbilityScriptableObject {
    [SerializeField] Vector3 Direction;
    [SerializeField] float Speed;
    [SerializeField] string AnimationName;
    [SerializeField] AttributeScriptableObject MovingVelocity;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new LegAbilityRightSpec(this, owner) {
            Direction = Direction,
            Speed = Speed,
            AnimationName = AnimationName,
            MovingVelocity= MovingVelocity
        };

        return spec;
    }

    public class LegAbilityRightSpec : RunnerAbilitySpec {
        public Vector3 Direction;
        public float Speed;
        public string AnimationName;
        public AttributeScriptableObject MovingVelocity;

        bool Active;

        BaseCoreComponent Body;
        ICharacterCtrl Character;
        Animator animator;
        bool addConfirm = false;

        public LegAbilityRightSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;
            Active = false;            
        }
        public override void CancelAbility() {
            Active = false;
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }
        protected override IEnumerator ActivateAbility() {

            while (Active) {

                if (Character.Landing) {
                    // animator.SetBool("Move", true);
                    animator.SetBool(AnimationName, true);
                    // animator.SetInteger("MoveRecord", animator.GetInteger("MoveRecord") + 1);
                    addConfirm = true;
                }
                while (Active && Character.Landing) {
                    if (Body.Root.AttributeSystemComponent.GetAttributeValue(MovingVelocity, out var s))
                    {
                        Speed = s.CurrentValue / 25;
                    }
                    // Character.Move(Body.BodyTransform.TransformDirection(Direction) * Speed);
                    Character.HorizontalMove(Body.BodyTransform.TransformDirection(Direction).x * Speed);
                    yield return null;
                }

                if(addConfirm) {
                    // animator.SetBool("Move", false);
                    // animator.SetInteger("MoveRecord", animator.GetInteger("MoveRecord") - 1);
                    animator.SetBool(AnimationName, false);
                    addConfirm = false;
                }
                yield return null;
            }
        }
        protected override IEnumerator PreActivate() {
            Character = Body.Root as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl");
            //Debug.Log(Body.Root.AttributeSystemComponent.Attributes.Count);
            Active = Character != null;
            addConfirm = false;
            animator.SetFloat("Speed", Speed / 2);
            yield return null;
        }
    }
}
