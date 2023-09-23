using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class InGameUI : NetworkBehaviour {
    [SerializeField] SkillBinder Binder;


    private BasePlayer player;
    private AbilityManager abilityManager;

    private async void Start() {
        // player = GetComponent<BasePlayer>();
        player = Utils.GetLocalPlayerDevice();
        
        await UniTask.WaitUntil(() => player.IsAlive.Value == true);
        if (IsServer) {
            abilityManager = player.SelfDevice.AbilityManager;
            GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (_, _) => RefreshAllSkillBox();
            GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += _ => RefreshAllSkillBox();

            RefreshAllSkillBox();
        }

        // Bind Actions
        Binder.setAbilityAction += BindAbilityToEntry;
    }


    void BindAbilityToEntry(int origin, int newID, int abilityID) {
        if(IsOwner) {
            BindAbilityToEntry_ServerRpc(origin,newID,abilityID);
        }
    }

    [ServerRpc]
    void BindAbilityToEntry_ServerRpc(int origin, int newID, int abilityID) {
        var ability = origin != -1 ? 
            abilityManager.AbilityInputEntries[origin].Abilities[abilityID] :
            abilityManager.GetAbilitiesOutOfEntry()[abilityID];
        if (newID == -1) { abilityManager.SetAbilityOutOfEntry(ability); } else { abilityManager.SetAbilityToEntry(newID, ability); }
    }

    void RefreshAllSkillBox() {
        if (IsServer) {
            int abilityID = 0;
            for (int i = 0; i < 10; ++i) {
                abilityID = 0;
                abilityManager.AbilityInputEntries[i].Abilities.ForEach(a => {
                    RefreshSkillBox_ClientRpc(i, abilityID, a.AbilityScriptableObject.AbilityName);
                    abilityID++;
                });
            }
            
            abilityID = 0;
            abilityManager.GetAbilitiesOutOfEntry().ForEach(a => {
                RefreshSkillBox_ClientRpc(-1, abilityID, a.AbilityScriptableObject.AbilityName);
                abilityID++;
            });
        }
    }

    [ClientRpc]
    void RefreshSkillBox_ClientRpc(int BoxID, int abilityID, string abilityName) {
        var ability = ResourceManager.Instance.GetAbilityScriptableObjectByName(abilityName);
        var DASO = ability as DisplayableAbilityScriptableObject;

        Binder.SetDisply(BoxID, abilityID,DASO);
    }
}
