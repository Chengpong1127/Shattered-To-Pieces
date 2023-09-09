using System.Collections;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability/AssemblyToggleAbility")]
public class AssemblyToggleAbility : DisplayableAbilityScriptableObject
{
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AssemblyToggleAbilitySpec(this, owner);
        return spec;
    }

    public class AssemblyToggleAbilitySpec : RunnerAbilitySpec
    {
        private ControlRoom controlRoom;
        public AssemblyToggleAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            controlRoom = SelfEntity as ControlRoom ?? throw new System.ArgumentNullException("The entity should have ControlRoom.");
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
            controlRoom.ToggleAssembly(Runner.OwnerPlayerID);
            yield return null;
        }
    }
}