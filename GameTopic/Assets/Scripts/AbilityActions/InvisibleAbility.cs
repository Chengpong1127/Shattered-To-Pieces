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
    [SerializeField]
    Material invisible;
    [SerializeField]
    Material m_default;
    protected float DurationTime=4.0f;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new InvisibleAbilitySpec(this, owner)
        {
            m_default = m_default,
            invisible = invisible,
            DurationTime=DurationTime
        };

        return spec;

    }
    protected class InvisibleAbilitySpec : RunnerAbilitySpec
    {
        public Material m_default;
        public Material invisible;
        public BaseCoreComponent ControlRoom;
        public float DurationTime;
        public BaseCoreComponent[] baseCoreComponents;
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
            ControlRoom = (SelfEntity as BaseCoreComponent).Root as ControlRoom;
            baseCoreComponents = ControlRoom.GetAllChildren();
            ClientRpcParams clientRpcParams = new ClientRpcParams(){
                Send = new ClientRpcSendParams(){
                    TargetClientIds = new List<ulong>() { Runner.OwnerPlayerID }
                }
            };
            baseCoreComponents.ToList().ForEach(baseCoreComponent =>
            {
                baseCoreComponent.SetVisible_ClientRpc(false);
            });
            baseCoreComponents.ToList().ForEach(baseCoreComponent =>
            {
                baseCoreComponent.SetVisible_ClientRpc(true, clientRpcParams);
            });
            yield return new WaitForSeconds(DurationTime);
            baseCoreComponents.ToList().ForEach(baseCoreComponent =>
            {
                baseCoreComponent.SetVisible_ClientRpc(true);
            });
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

