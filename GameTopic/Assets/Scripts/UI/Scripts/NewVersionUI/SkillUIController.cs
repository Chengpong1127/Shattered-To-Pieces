using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillUIController : NetworkBehaviour {

    [SerializeField] SkillDropper NonDropper;
    [SerializeField] List<SkillDropper> Droppers;
    [SerializeField]
    private BasePlayer player;
    private AbilityManager abilityManager;

    private void Awake() {
        Debug.Assert(NonDropper != null);
        Debug.Assert(Droppers != null);
        Debug.Assert(player != null);


        NonDropper.OnAddNewSkill += BindAbilityToEntry;
        NonDropper.draggerList.ForEach(d => {
            d.NonSetDropper = NonDropper;
            d.DraggingParentTransform = this.transform.parent;
        });
        Droppers.ForEach(d => {
            d.draggerList.ForEach(d => {
                d.NonSetDropper = NonDropper;
                d.DraggingParentTransform = this.transform.parent;
            });
            d.OnAddNewSkill += BindAbilityToEntry;
            d.SelfBoxID = Droppers.IndexOf(d);
            d.OnRebindStart += RebindKeyText;
        });
    }

    private void Start() {
        LoadUI();
        GameEvents.AbilityManagerEvents.OnSetBinding += (eID, _) => UpdateSkillBoxKeyText(eID);
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated += ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (am, _, _) => ServerRequestUpdateAllSkillBox(am);
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += (am, _) => ServerRequestUpdateAllSkillBox(am);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        GameEvents.AbilityManagerEvents.OnSetBinding -= (eID, _) => UpdateSkillBoxKeyText(eID);
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated -= ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry -= (am, _, _) => ServerRequestUpdateAllSkillBox(am);
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry -= (am, _) => ServerRequestUpdateAllSkillBox(am);
    }

    public async UniTask Initialize(){
        // init each dropper animation every 0.1s
        foreach (var dropper in Droppers) {
            dropper.InitializeAnimation().Forget();
            await UniTask.Delay(50);
        }
    }

    public void ShowSkillUI(){
        Droppers.ForEach(d => d.Show());
        NonDropper.Show();
    }
    public void HideSkillUI(){
        Droppers.ForEach(d => d.Hide());
        NonDropper.Hide();
    }

    private void ServerRequestUpdateAllSkillBox(AbilityManager updatedAbilityManager){
        if (abilityManager == updatedAbilityManager){
            RefreshAllSkillBox_ServerRpc();
        }
    }

    public void LoadUI(){
        if (IsOwner) {
            RefreshAllSkillBox_ServerRpc();
            UpdateAllSkillBoxKeyText_ServerRpc();
        }
    }

    [ServerRpc]
    private void UpdateAllSkillBoxKeyText_ServerRpc() {
        for(int i = 0; i < 10; ++i) {
            UpdateSkillBoxKeyText(i);
        }
    }

    private void SetDisplay(int boxID, int abilityID, DisplayableAbilityScriptableObject DASO, ulong ownerID) {
        GameComponent owner = DASO == null? null : Utils.GetLocalGameObjectByNetworkID(ownerID).GetComponent<GameComponent>();
        if (boxID == -1) { NonDropper.SetDisplay(abilityID, DASO, owner); }
        else { Droppers[boxID].SetDisplay(abilityID, DASO, owner); }
    }
    private void BindAbilityToEntry(int origin, int newID, int abilityID) {
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


        this.SetDisplay(BoxID, abilityID, DASO, ownerID);
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
        if(IsOwner) {
            var gamePlayer = player as GamePlayer;
            gamePlayer?.AbilityRebinder.StartRebinding(entryID);
        }
        
    }
}
