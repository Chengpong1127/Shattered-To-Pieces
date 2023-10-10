using System.Collections;
using System.Linq;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability/AssemblyToggleAbility")]
public class AssemblyToggleAbility : DisplayableAbilityScriptableObject
{
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AssemblyToggleAbilitySpec(this, owner){
        };
        return spec;
    }

    public class AssemblyToggleAbilitySpec : RunnerAbilitySpec
    {
        private bool isAssembly = false;
        public AssemblyToggleAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            return;
        }
        protected override IEnumerator ActivateAbility()
        {
            var player = Utils.ServerGetBasePlayer(Runner.OwnerPlayerID) as AssemblyablePlayer;
            isAssembly = !isAssembly;
            player.SetAssemblyMode_ClientRpc(isAssembly);
            yield return null;
        }
    }
}