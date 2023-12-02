using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using DG.Tweening;
using System.Linq;
using Unity.Netcode;
[CreateAssetMenu(fileName = "InvisibleAbility", menuName = "Ability/InvisibleAbility")]
public class InvisibleAbility : DisplayableAbilityScriptableObject
{
    protected float DurationTime=4.0f;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new InvisibleAbilitySpec(this, owner)
        {
            DurationTime=DurationTime
        };

        return spec;

    }
    protected class InvisibleAbilitySpec : RunnerAbilitySpec
    {
        public float DurationTime;
        public List<BaseCoreComponent> baseCoreComponents;
        public InvisibleAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
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
            if (SelfEntity is BaseCoreComponent baseCoreComponent){
                var deviceComponents = baseCoreComponent.Root.GetAllChildrenList();
                deviceComponents.Add(baseCoreComponent.Root);
                ClientRpcParams ownerParams = new ClientRpcParams(){
                    Send = new ClientRpcSendParams(){
                        TargetClientIds = new List<ulong>() { Runner.OwnerPlayerID }
                    }
                };
                var allPlayerIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
                ClientRpcParams otherParams = new ClientRpcParams(){
                    Send = new ClientRpcSendParams(){
                        TargetClientIds = allPlayerIds.Except(new List<ulong>() { Runner.OwnerPlayerID }).ToList()
                    }
                };
                deviceComponents.ForEach(deviceComponent =>
                {
                    deviceComponent.SetAlpha_ClientRpc(0, 0.5f, otherParams);
                });
                deviceComponents.ForEach(deviceComponent =>
                {
                    deviceComponent.SetAlpha_ClientRpc(0.5f, 0.5f, ownerParams);
                });
                yield return new WaitForSeconds(DurationTime);
                deviceComponents.ForEach(deviceComponent =>
                {
                    deviceComponent.SetAlpha_ClientRpc(1, 0.5f);
                });

            }
        }
    }
}

