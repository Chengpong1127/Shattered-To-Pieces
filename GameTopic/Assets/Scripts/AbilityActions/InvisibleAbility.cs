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
            ControlRoom = (SelfEntity as BaseCoreComponent).Root as ControlRoom;
            baseCoreComponents = ControlRoom.GetAllChildrenList();
            baseCoreComponents.Add((SelfEntity as BaseCoreComponent).Root);
            ClientRpcParams clientRpcParams = new ClientRpcParams(){
                Send = new ClientRpcSendParams(){
                    TargetClientIds = new List<ulong>() { Runner.OwnerPlayerID }
                }
            };
            baseCoreComponents.ForEach(baseCoreComponent =>
            {
                baseCoreComponent.SetVisible_ClientRpc(false,1);
            });
            baseCoreComponents.ForEach(baseCoreComponent =>
            {
                baseCoreComponent.SetVisible_ClientRpc(true, 0.5f, clientRpcParams);
            });
            yield return new WaitForSeconds(DurationTime);
            baseCoreComponents.ForEach(baseCoreComponent =>
            {
                baseCoreComponent.SetVisible_ClientRpc(true,1);
            });
            yield return null;
        }
    }
}

