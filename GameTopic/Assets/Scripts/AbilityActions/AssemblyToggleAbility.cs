using System.Collections;
using System.Linq;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;
[CreateAssetMenu(menuName = "Ability/AssemblyToggleAbility")]
public class AssemblyToggleAbility : DisplayableAbilityScriptableObject
{
    public GameplayEffectScriptableObject AssemblyEffect;
    public GameplayEffectScriptableObject AssemblyCancelEffect;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new AssemblyToggleAbilitySpec(this, owner){
            AssemblyEffect = AssemblyEffect,
            AssemblyCancelEffect = AssemblyCancelEffect
        };
        return spec;
    }

    public class AssemblyToggleAbilitySpec : RunnerAbilitySpec
    {
        private ControlRoom controlRoom;
        private bool isAssembly = false;
        public GameplayEffectScriptableObject AssemblyEffect;
        public GameplayEffectScriptableObject AssemblyCancelEffect;
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
            var player = Utils.ServerGetBasePlayer(Runner.OwnerPlayerID) as AssemblyablePlayer;
            isAssembly = !isAssembly;
            player.SetAssemblyMode_ClientRpc(isAssembly);
            Debug.Log("AssemblyToggleAbilitySpec: " + isAssembly);
            if (isAssembly)
            {
                controlRoom.GetAllChildren().ToList().ForEach(child => {
                    GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(controlRoom, child, AssemblyEffect);
                });
            }
            else
            {
                controlRoom.GetAllChildren().ToList().ForEach(child => {
                    GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(controlRoom, child, AssemblyCancelEffect);
                });
            }
            yield return null;
        }
    }
}