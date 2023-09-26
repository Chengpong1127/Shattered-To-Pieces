using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillBinder : NetworkBehaviour {
    [SerializeField] Button EditBTN;

    [SerializeField] SkillDropper NonDropper;
    [SerializeField] List<SkillDropper> Droppers;
    public UnityAction<int, int, int> setAbilityAction { get; set; }
    bool firstTimeRunOnEnable = true;


    // set entry stuff.
    private BasePlayer player;
    private AbilityManager abilityManager;

    private void Awake() {
        // Dropper setting.
        NonDropper.Binder = this;
        NonDropper.draggerList.ForEach(d => {
            d.NonSetDropper = NonDropper;
            d.DraggingParentTransform = this.transform.parent;
            d.UpdateDisplay(null);
        });
        int id = 0;
        Droppers.ForEach(d => {
            d.draggerList.ForEach(d => {
                d.NonSetDropper = NonDropper;
                d.DraggingParentTransform = this.transform.parent;
                d.UpdateDisplay(null);
            });
            d.Binder = this;
            d.BoxID = id;
            id++;
        });

        // Bind Actions
        this.setAbilityAction += BindAbilityToEntry;

        //this.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private async void OnEnable() {
        // Debug.Log("IsServer: " + IsServer + " IsClient: " + IsClient + " IsOwner: " + IsOwner);
        // set entry stuff
        // player = GetComponent<BasePlayer>();
        player = GetComponentInParent<BasePlayer>();

        //await UniTask.WaitUntil(() => player.IsAlive.Value == true);
        if (IsServer && firstTimeRunOnEnable) {
            firstTimeRunOnEnable = false;
            abilityManager = player.SelfDevice.AbilityManager;
            GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (_, _) => RefreshAllSkillBox();
            GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += _ => RefreshAllSkillBox();

            RefreshAllSkillBox();
        }
    }

    // private void OnEnable() {
    //     if (IsServer) {
    //         Debug.Log("SkillBinder Call enable on server.");
    //         RefreshAllSkillBox();
    //     }
    // }

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
        var ability = origin != -1 ?
            abilityManager.AbilityInputEntries[origin].Abilities[abilityID] :
            abilityManager.GetAbilitiesOutOfEntry()[abilityID];
        if (newID == -1) { abilityManager.SetAbilityOutOfEntry(ability); } else { abilityManager.SetAbilityToEntry(newID, ability); }
    }

    void RefreshAllSkillBox() {
        if (IsServer) {
            // clear all skills
            NonDropper.draggerList.ForEach(d => {
                d.UpdateDisplay(null);
            });
            Droppers.ForEach(d => {
                d.draggerList.ForEach(d => {
                    d.UpdateDisplay(null);
                });
            });

            // show skills
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

        // Debug.Log("AbilityName : " + abilityName + " get ability : " + ability != null);

        this.SetDisply(BoxID, abilityID, DASO);
    }
}
