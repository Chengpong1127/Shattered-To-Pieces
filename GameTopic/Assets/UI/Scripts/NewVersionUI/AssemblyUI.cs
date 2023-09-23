using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class AssemblyUI : NetworkBehaviour {
    IAssemblyRoom room;

    [SerializeField] SkillBinder Binder;


    private async void Start() {
        GameObject impRoom = GameObject.Find("RoomManager");
        var roomRunner = impRoom.GetComponent<AssemblyRoomRunner>();
        await UniTask.WaitUntil(() => roomRunner.StateMachine.State == AssemblyRoomRunner.GameStates.Gaming);
        room = impRoom.GetComponent<IAssemblyRoom>();

        // Bind Actions
        Binder.setAbilityAction += BindAbilityToEntry;

        RefreshAllSkillBox();
    }

    void BindAbilityToEntry(int origin, int newID, int abilityID) {
        var ability = origin != -1 ?
            room.AbilityManager.AbilityInputEntries[origin].Abilities[abilityID] :
            room.AbilityManager.GetAbilitiesOutOfEntry()[abilityID];

        if (newID == -1) { room.AbilityManager.SetAbilityOutOfEntry(ability); }
        else { room.AbilityManager.SetAbilityToEntry(newID, ability); }
        
        List<DisplayableAbilityScriptableObject> DASOlst;
        DASOlst = origin != -1 ?
            room.AbilityManager.AbilityInputEntries[origin].Abilities.Where(a => true).Select(a => a.AbilityScriptableObject as DisplayableAbilityScriptableObject).ToList() :
            room.AbilityManager.GetAbilitiesOutOfEntry().Where(a => true).Select(a => a.AbilityScriptableObject as DisplayableAbilityScriptableObject).ToList();
        Binder.SetDisply(origin, DASOlst);

        DASOlst = newID != -1 ?
            room.AbilityManager.AbilityInputEntries[newID].Abilities.Where(a => true).Select(a => a.AbilityScriptableObject as DisplayableAbilityScriptableObject).ToList() :
            room.AbilityManager.GetAbilitiesOutOfEntry().Where(a => true).Select(a => a.AbilityScriptableObject as DisplayableAbilityScriptableObject).ToList();
        Binder.SetDisply(newID, DASOlst);
    }

    void RefreshAllSkillBox() {
        for (int i = 0; i < 10; ++i) {
            Binder.SetDisply(i, room.AbilityManager.AbilityInputEntries[i].Abilities.Where(a => true).Select(a => a.AbilityScriptableObject as DisplayableAbilityScriptableObject).ToList());
        }
        Binder.SetDisply(-1, room.AbilityManager.GetAbilitiesOutOfEntry().Where(a => true).Select(a => a.AbilityScriptableObject as DisplayableAbilityScriptableObject).ToList());
    }
}
