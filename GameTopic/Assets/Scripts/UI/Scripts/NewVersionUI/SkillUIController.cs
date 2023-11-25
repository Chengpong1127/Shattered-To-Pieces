using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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

        GameEvents.AbilityManagerEvents.OnSetBinding += OnSetBindingHandler;
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated += ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += OnSetAbilityToEntryHandler;
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += OnSetAbilityOutOfEntryHandler;

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


    public override void OnDestroy() {
        GameEvents.AbilityManagerEvents.OnSetBinding -= OnSetBindingHandler;
        GameEvents.AbilityManagerEvents.OnAbilityManagerUpdated -= ServerRequestUpdateAllSkillBox;
        GameEvents.AbilityManagerEvents.OnSetAbilityToEntry -= OnSetAbilityToEntryHandler;
        GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry -= OnSetAbilityOutOfEntryHandler;
        base.OnDestroy();
    }

    private void OnSetBindingHandler(int entryID, string path) {
        UpdateSkillBoxKeyText(entryID);
    }

    private void OnSetAbilityToEntryHandler(AbilityManager abilityManager, GameComponentAbility ability, int entryID) {
        ServerRequestUpdateAllSkillBox(abilityManager);
    }

    private void OnSetAbilityOutOfEntryHandler(AbilityManager abilityManager, GameComponentAbility ability) {
        ServerRequestUpdateAllSkillBox(abilityManager);
    }

    public async UniTask Initialize(){
        foreach (var dropper in Droppers) {
            dropper.InitializeAnimation().Forget();
            await UniTask.Delay(100);
        }
        NonDropper.InitializeAnimation().Forget();
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
        if (abilityManager == updatedAbilityManager)
            RefreshAllSkillBox_ServerRpc();
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

            // TODO: Predict the ability binding.
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
        abilityManager = player.SelfDevice.AbilityManager;
        ClearAllSkills();
        ShowSkills();
    }

    void ClearAllSkills() {
        NonDropper.draggerList.ForEach(d => {
            RefreshSkillBox_ClientRpc(-1, NonDropper.draggerList.IndexOf(d), null, 0);
        });
        Droppers.ForEach(dropper => {
            dropper.draggerList.ForEach(dragger => {
                RefreshSkillBox_ClientRpc(Droppers.IndexOf(dropper), dropper.draggerList.IndexOf(dragger), null, 0);
            });
        });
    }

    void ShowSkills() {
        int abilityID = 0;

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
