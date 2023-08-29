using AbilitySystem;
using AbilitySystem.Authoring;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RotationAbility;
using UnityEngine.SocialPlatforms;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

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

    public class LegAbilityRightSpec : EntityAbilitySpec {
        public Vector3 Direction;
        public float Speed;
        bool Active;

        BaseCoreComponent Body;

        public LegAbilityRightSpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner) {
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
            while(Active) {
                Body.Root.BodyTransform.Translate(Direction* Speed * Time.fixedDeltaTime, Space.Self);
                yield return null;
            }

            yield return null;
        }
        protected override IEnumerator PreActivate() {
            Active = true;
            yield return null;
        }
    }
}
