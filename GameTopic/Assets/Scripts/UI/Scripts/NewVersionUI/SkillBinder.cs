using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillBinder : NetworkBehaviour {

    [SerializeField] SkillDropper NonDropper;
    [SerializeField] List<SkillDropper> Droppers;
    public UnityAction<int, int, int> setAbilityAction { get; set; }
    // set entry stuff.
    [SerializeField]
    private BasePlayer player;
    private AbilityManager abilityManager;
    private AbilityRebinder rebinder;
    public override void OnDestroy() {
        base.OnDestroy();
        GameEvents.AbilityManagerEvents.OnSetBinding -= (eID, _) => UpdateSkillBoxKeyText(eID);
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated -= ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry -= (am, _, _) => ServerRequestUpdateAllSkillBox(am);
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry -= (am, _) => ServerRequestUpdateAllSkillBox(am);
    }
    private void ServerRequestUpdateAllSkillBox(AbilityManager updatedAbilityManager){
        if (abilityManager == updatedAbilityManager){
            RefreshAllSkillBox_ServerRpc();
        }
    }

    private void Awake() {
        NonDropper.Binder = this;
        NonDropper.draggerList.ForEach(d => {
            d.NonSetDropper = NonDropper;
            d.DraggingParentTransform = this.transform.parent;
        });
        int id = 0;
        Droppers.ForEach(d => {
            d.draggerList.ForEach(d => {
                d.NonSetDropper = NonDropper;
                d.DraggingParentTransform = this.transform.parent;
            });
            d.Binder = this;
            d.BoxID = id;
            d.RebindBTN.onClick.AddListener(() => RebindKeyText(d.BoxID));
            id++;
        });
    }

    private void Start() {
        // Bind Actions
        this.setAbilityAction += BindAbilityToEntry;
        GameEvents.AbilityManagerEvents.OnSetBinding += (eID, _) => UpdateSkillBoxKeyText(eID);
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated += ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (am, _, _) => ServerRequestUpdateAllSkillBox(am);
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += (am, _) => ServerRequestUpdateAllSkillBox(am);
    }

    public void LoadUI(){
        if (IsOwner) {
            RefreshAllSkillBox_ServerRpc();
            UpdateAllSkillBoxKeyText_ServerRpc();
        }
    }

    private void OnEnable() {
        LoadUI();
    }
    [ServerRpc]
    private void UpdateAllSkillBoxKeyText_ServerRpc() {
        for(int i = 0; i < 10; ++i) {
            UpdateSkillBoxKeyText(i);
        }
    }
    public void Bind(int origin, int newID, int abilityID) {
        setAbilityAction?.Invoke(origin, newID, abilityID);
    }

    public void SetDisply(int boxID, int abilityID, DisplayableAbilityScriptableObject DASO, ulong ownerID) {
        GameComponent owner = DASO == null? null : Utils.GetLocalGameObjectByNetworkID(ownerID).GetComponent<GameComponent>();
        if (boxID == -1) { NonDropper.SetDisplay(abilityID, DASO, owner); }
        else { Droppers[boxID].SetDisplay(abilityID, DASO, owner); }
    }


    // set entry stuff.
    void BindAbilityToEntry(int origin, int newID, int abilityID) {
        if (IsOwner) {
            BindAbilityToEntry_ServerRpc(origin, newID, abilityID);
        }
    }

    [ServerRpc]
    void BindAbilityToEntry_ServerRpc(int origin, int newID, int abilityID) {
        var ability = origin != -1 ?
            abilityManager.AbilityInputEntries[origin].Abilities[abilityID] :
            abilityManager.GetAbilitiesOutOfEntry()[abilityID];
        if (newID == -1) { abilityManager.SetAbilityOutOfEntry(ability); } else { abilityManager.SetAbilityToEntry(newID, ability); }
    }
    [ServerRpc(RequireOwnership = false)]
    void RefreshAllSkillBox_ServerRpc() {
        if (IsServer) {
            abilityManager = player.SelfDevice.AbilityManager;

            // clear all skills
            int abilityID = 0;
            int entryID = 0;
            NonDropper.draggerList.ForEach(d => {
                RefreshSkillBox_ClientRpc(-1, abilityID, null, 0);
                abilityID++;
            });
            Droppers.ForEach(d => {
                abilityID = 0;
                d.draggerList.ForEach(d => {
                    RefreshSkillBox_ClientRpc(entryID, abilityID, null, 0);
                    abilityID++;
                });
                entryID++;
            });

            // show skills
            abilityID = 0;
            for (int i = 0; i < 10; ++i) {
                abilityID = 0;
                abilityManager.AbilityInputEntries[i].Abilities.ForEach(a => {
                    RefreshSkillBox_ClientRpc(i, abilityID, a.AbilityScriptableObject.AbilityName, a.OwnerGameComponentID);
                    abilityID++;
                });
            }

            abilityID = 0;
            abilityManager.GetAbilitiesOutOfEntry().ForEach(a => {
                RefreshSkillBox_ClientRpc(-1, abilityID, a.AbilityScriptableObject.AbilityName, a.OwnerGameComponentID);
                abilityID++;
            });
        }
    }

    [ClientRpc]
    void RefreshSkillBox_ClientRpc(int BoxID, int abilityID, string abilityName, ulong ownerID) {
        if(!IsOwner) { return; }
        var ability = abilityName != null ? ResourceManager.Instance.GetAbilityScriptableObjectByName(abilityName) : null;
        var DASO = ability as DisplayableAbilityScriptableObject;

        // Debug.Log("AbilityName : " + abilityName + " get ability : " + ability != null);

        this.SetDisply(BoxID, abilityID, DASO, ownerID);
    }

    void UpdateSkillBoxKeyText(int entryID) {
        if (GameRunner.ServerGameRunnerInstance.StateMachine.State == BaseGameRunner.GameStates.Gaming) {
            var keyText = abilityManager != null ? abilityManager.AbilityInputEntries[entryID].InputPath : "Non";
            SetSkillBoxKeyText_ClientRpc(entryID, keyText);
        }

    }
    [ClientRpc]
    void SetSkillBoxKeyText_ClientRpc(int entryID, string keyStr) {
        if (IsOwner){
            Droppers[entryID].SetKeyText(keyStr);
        }
    }

    void RebindKeyText(int entryID) {
        if(!IsOwner) { return; }
        var gamePlayer = player as GamePlayer;
        rebinder = gamePlayer?.AbilityRebinder;

        rebinder.StartRebinding(entryID);
    }
}
