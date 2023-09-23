using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InGameUI : NetworkBehaviour {
    [SerializeField] SkillBinder Binder;


    private BasePlayer player;
    private AbilityManager abilityManager;

    private async void Awake() {
        player = GetComponent<BasePlayer>();
        await UniTask.WaitUntil(() => player.IsAlive.Value == true);
        if (IsServer) {
            abilityManager = player.SelfDevice.AbilityManager;
            GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (_, _) => RefreshAllSkillBox();
            GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += _ => RefreshAllSkillBox();

            RefreshAllSkillBox();
        }
    }

    private void Start() {
        // Bind Actions
        Binder.setAbilityAction += BindAbilityToEntry;
    }


    void BindAbilityToEntry(int origin, int newID, GameComponentAbility ability) {
        if(IsOwner) {
            // BindAbilityToEntry_ServerRpc(origin,newID,ability);
        }

        // Local bind
        // if (newID == -1) { abilityManager.SetAbilityOutOfEntry(ability); } else { abilityManager.SetAbilityToEntry(newID, ability); }
        // 
        // Binder.SetDisply(origin, origin != -1 ? abilityManager.AbilityInputEntries[origin].Abilities : abilityManager.GetAbilitiesOutOfEntry());
        // Binder.SetDisply(newID, newID != -1 ? abilityManager.AbilityInputEntries[newID].Abilities : abilityManager.GetAbilitiesOutOfEntry());
    }

    // [ServerRpc]
    // void BindAbilityToEntry_ServerRpc(int origin, int newID, Sprite ability) {
    //     if (newID == -1) { abilityManager.SetAbilityOutOfEntry(ability); } else { abilityManager.SetAbilityToEntry(newID, ability); }
    // }

    void RefreshAllSkillBox() {
        if (IsServer) {
            for (int i = 0; i < 10; ++i) {
                // RefreshSkillBox_ClientRpc(i, abilityManager.AbilityInputEntries[i].Abilities);
            }
            // RefreshSkillBox_ClientRpc(-1, abilityManager.GetAbilitiesOutOfEntry());
        }
        // Local Update
        // for (int i = 0; i < 10; ++i) {
        //     Binder.SetDisply(i, abilityManager.AbilityInputEntries[i].Abilities);
        // }
        // Binder.SetDisply(-1, abilityManager.GetAbilitiesOutOfEntry());
    }

    // [ClientRpc]
    // void RefreshSkillBox_ClientRpc(int BoxID, List<GameComponentAbility> abilities) {
    //     Binder.SetDisply(BoxID, abilities);
    // }
}
