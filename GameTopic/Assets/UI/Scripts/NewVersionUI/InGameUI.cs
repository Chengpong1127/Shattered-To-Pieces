using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InGameUI : NetworkBehaviour {
    IAssemblyRoom room;
    [SerializeField] SkillBinder Binder;


    private BasePlayer player;
    private AbilityManager abilityManager;

    private async void Awake() {
        player = GetComponent<BasePlayer>();
        await UniTask.WaitUntil(() => player.IsAlive.Value == true);
        if (IsServer) {
            abilityManager = player.SelfDevice.AbilityManager;
            GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (_, _) => { };// put function at here
            GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += _ => { };// put function at here
        }
    }

    private async void Start() {
        GameObject impRoom = GameObject.Find("RoomManager");
        var roomRunner = impRoom.GetComponent<AssemblyRoomRunner>();
        await UniTask.WaitUntil(() => roomRunner.StateMachine.State == AssemblyRoomRunner.GameStates.Gaming);
        room = impRoom.GetComponent<IAssemblyRoom>();

        // Bind Actions
        Binder.setAbilityAction += BindAbilityToEntry;

        RefreshAllSkillBox();
    }


    void BindAbilityToEntry(int origin, int newID, GameComponentAbility ability) {

        if (newID == -1) { room.AbilityManager.SetAbilityOutOfEntry(ability); } else { room.AbilityManager.SetAbilityToEntry(newID, ability); }

        Binder.SetDisply(origin, origin != -1 ? room.AbilityManager.AbilityInputEntries[origin].Abilities : room.AbilityManager.GetAbilitiesOutOfEntry());
        Binder.SetDisply(newID, newID != -1 ? room.AbilityManager.AbilityInputEntries[newID].Abilities : room.AbilityManager.GetAbilitiesOutOfEntry());
    }

    void RefreshAllSkillBox() {
        for (int i = 0; i < 10; ++i) {
            Binder.SetDisply(i, room.AbilityManager.AbilityInputEntries[i].Abilities);
        }
        Binder.SetDisply(-1, room.AbilityManager.GetAbilitiesOutOfEntry());
    }
}
