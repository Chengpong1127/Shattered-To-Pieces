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
    [SerializeField] Button EditBTN;

    [SerializeField] SkillDropper NonDropper;
    [SerializeField] List<SkillDropper> Droppers;
    public UnityAction<int, int, int> setAbilityAction { get; set; }
    // set entry stuff.
    private BasePlayer player;
    private AbilityManager abilityManager;
    private GamePlayer gamePlayer;
    private AbilityRebinder rebinder;
    void Awake()
    {
        GameEvents.AbilityManagerEvents.OnSetBinding += (eID, _) => UpdateSkillBoxKeyText(eID);
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated += ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (am, _, _) => ServerRequestUpdateAllSkillBox(am);
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += (am, _) => ServerRequestUpdateAllSkillBox(am);
    }
    private void ServerRequestUpdateAllSkillBox(AbilityManager updatedAbilityManager){
        if (abilityManager == updatedAbilityManager){
            RefreshAllSkillBox_ServerRpc();
        }
    }

    private void Start() {
        // Dropper setting.
        NonDropper.Binder = this;
        NonDropper.draggerList.ForEach(d => {
            d.NonSetDropper = NonDropper;
            d.DraggingParentTransform = this.transform.parent;
            // d.UpdateDisplay(null);
        });
        int id = 0;
        Droppers.ForEach(d => {
            d.draggerList.ForEach(d => {
                d.NonSetDropper = NonDropper;
                d.DraggingParentTransform = this.transform.parent;
                // d.UpdateDisplay(null);
            });
            d.Binder = this;
            d.BoxID = id;
            d.RebindBTN.onClick.AddListener(() => RebindKeyText(d.BoxID));
            id++;
        });

        // Bind Actions
        this.setAbilityAction += BindAbilityToEntry;

        //this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void OnEnable() {
        if (IsOwner){
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

    

    public void Bind(int origin, int newID, int abilityID) {
        setAbilityAction?.Invoke(origin, newID, abilityID);
    }

    public void SetDisply(int id, List<DisplayableAbilityScriptableObject> abilities) {
        if(id == -1) {
            NonDropper.SetDisplay(abilities);
        } else if (Droppers.Count > id) {
            Droppers[id].SetDisplay(abilities);
        }
    }

    public void SetDisply(int boxID, int abilityID, DisplayableAbilityScriptableObject DASO) {
        if (boxID == -1) { NonDropper.SetDisplay(abilityID, DASO); }
        else { Droppers[boxID].SetDisplay(abilityID, DASO); }
    }


    // set entry stuff.
    void BindAbilityToEntry(int origin, int newID, int abilityID) {
        if (IsOwner) {
            BindAbilityToEntry_ServerRpc(origin, newID, abilityID);
        }
    }

    [ServerRpc]
    void BindAbilityToEntry_ServerRpc(int origin, int newID, int abilityID) {
        Debug.Log(newID + " " + abilityID + " " + origin);
        var ability = origin != -1 ?
            abilityManager.AbilityInputEntries[origin].Abilities[abilityID] :
            abilityManager.GetAbilitiesOutOfEntry()[abilityID];
        if (newID == -1) { abilityManager.SetAbilityOutOfEntry(ability); } else { abilityManager.SetAbilityToEntry(newID, ability); }
    }
    [ServerRpc(RequireOwnership = false)]
    void RefreshAllSkillBox_ServerRpc() {
        if (IsServer) {
            player = GetComponentInParent<BasePlayer>();
            abilityManager = player.SelfDevice.AbilityManager;

            // clear all skills
            int abilityID = 0;
            int entryID = 0;
            NonDropper.draggerList.ForEach(d => {
                RefreshSkillBox_ClientRpc(-1, abilityID, null);
                abilityID++;
            });
            Droppers.ForEach(d => {
                abilityID = 0;
                d.draggerList.ForEach(d => {
                    RefreshSkillBox_ClientRpc(entryID, abilityID, null);
                    abilityID++;
                });
                entryID++;
            });

            // show skills
            abilityID = 0;
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
        if(!IsOwner) { return; }
        var ability = abilityName != null ? ResourceManager.Instance.GetAbilityScriptableObjectByName(abilityName) : null;
        var DASO = ability as DisplayableAbilityScriptableObject;

        // Debug.Log("AbilityName : " + abilityName + " get ability : " + ability != null);

        this.SetDisply(BoxID, abilityID, DASO);
    }

    void UpdateSkillBoxKeyText(int entryID) {
        var keyText = abilityManager != null ? abilityManager.AbilityInputEntries[entryID].InputPath : "Non";

        SetSkillBoxKeyText_ClientRpc(entryID, keyText);
    }
    [ClientRpc]
    void SetSkillBoxKeyText_ClientRpc(int entryID, string keyStr) {
        if(!IsOwner) { return; }
        SetSkillBoxKeyText(entryID, keyStr);
    }
    void SetSkillBoxKeyText(int entryID, string keyStr) {
        if(Droppers.Count <= entryID) { return; }
        var result = keyStr.Split('/');
        Droppers[entryID].BindingKeyText.text = result[result.Length - 1];
        // var result2 = keyStr != null ? InputControlPath.ToHumanReadableString(keyStr) : "";
        // Droppers[entryID].BindingKeyText.text = result2;
    }

    void RebindKeyText(int entryID) {
        // AbilityRebinder.StartRebinding(entryID);
        if(!IsOwner) { return; }
        gamePlayer = GetComponentInParent<GamePlayer>();
        rebinder = gamePlayer?.AbilityRebinder;
        if(rebinder == null) { return; }

        rebinder.StartRebinding(entryID);
    }
}
