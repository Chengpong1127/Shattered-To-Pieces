using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "AimAbility", menuName = "Ability/AimAbility")]
public class AimAbility: DisplayableAbilityScriptableObject{
    [SerializeField]
    protected EndTriggerType endTriggerType;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AimAbilitySpec(this, owner)
        {
            triggerType = endTriggerType
        };
        return spec;
    }
    public class AimAbilitySpec : RunnerAbilitySpec
    {
        public EndTriggerType triggerType;
        private IAimable Aimable;
        public AimAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            Aimable = SelfEntity as IAimable ?? throw new System.ArgumentNullException("The entity should implement IAimable");
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
            Aimable.StartAim(GetCurrentAimVector());
            switch (triggerType)
            {
                case EndTriggerType.Press:
                    yield return new WaitUntil(() => Mouse.current.leftButton.isPressed);
                    break;
                case EndTriggerType.Release:
                    yield return new WaitUntil(() => Mouse.current.leftButton.wasReleasedThisFrame);
                    break;
                default:
                    break;
            }
            Aimable.EndAim(GetCurrentAimVector());
            EndAbility();
        }
        private Vector2 GetCurrentAimVector()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            Vector2 startPoint = Camera.main.WorldToScreenPoint(Aimable.AimStartPoint);
            return mousePosition - startPoint;
        }
    }
    public enum EndTriggerType
    {
        Press,
        Release,
    }
}