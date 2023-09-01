using AbilitySystem;
using AbilitySystem.Authoring;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "LegAbilityRight", menuName = "Ability/LegAbilityRight")]
public class LegAbilityRight : AbstractAbilityScriptableObject {
    [SerializeField] Vector3 Direction;
    [SerializeField] float Speed;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner) {
        var spec = new LegAbilityRightSpec(this, owner) {
            Direction = Direction,
            Speed = Speed
        };

        return spec;
    }

    public class LegAbilityRightSpec : RunnerAbilitySpec {
        public Vector3 Direction;
        public float Speed;
        bool Active;

        BaseCoreComponent Body;
        ICharacterCtrl Character;
        Animator animator;

        public LegAbilityRightSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
            animator = (SelfEntity as BaseCoreComponent)?.BodyAnimator ?? throw new System.ArgumentNullException("The entity should have animator.");
            var obj = SelfEntity as IBodyControlable ?? throw new System.ArgumentNullException("SelfEntity");
            Body = obj.body;
            Active = false;
        }
        public override void CancelAbility() {
            Active = false;
            animator.SetBool("Move", false);
            return;
        }

        public override bool CheckGameplayTags() {
            return true;
        }
        protected override IEnumerator ActivateAbility() {
            if(Active && Character.Landing) {
                animator.SetBool("Move", true);
            }

            while (Active && Character.Landing) {

                // !Move
                // Body.Root.BodyRigidbody.AddForce(
                //     Body.BodyTransform.TransformDirection(Direction) * Speed
                // ) ;
                Character.Move(Body.BodyTransform.TransformDirection(Direction) * Speed, ForceMode2D.Force);
                yield return null;
            }

            animator.SetBool("Move", false);
            yield return null;
        }
        protected override IEnumerator PreActivate() {
            Character = Body.Root as ICharacterCtrl ?? throw new System.ArgumentNullException("Root component need ICharacterCtrl");

            Active = Character != null;
            animator.SetFloat("Speed", Speed);
            yield return null;
        }
    }
}
