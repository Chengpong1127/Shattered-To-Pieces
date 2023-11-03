using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class WorkShop : MonoBehaviour
{
    AssemblyRoomMode roomMode = AssemblyRoomMode.PlayMode;

    [SerializeField] IAssemblyRoom room;
    [SerializeField] PriceCtrl userDisplayMoney;
    [SerializeField] Button shoppingBTN;
    [SerializeField] Button settingBTN;
    [SerializeField] Button storeBTN;
    [SerializeField] Button loladBTN;
    [SerializeField] Button exitBTN;
    [SerializeField] ShopBGCtrl shopPage;
    [SerializeField] StoreFileCtrl fileCtrl;
    [SerializeField] SkillDispatcher shopDispatcher;

    private void Awake() {
        // Debug.Log(Application.persistentDataPath);
        // roomMode = AssemblyRoomMode.PlayMode;
        shoppingBTN.onClick.AddListener(SwitchRoomMode);
    }

    private async void Start() {
        GameObject impRoom = GameObject.Find("RoomManager");
        var room = impRoom.GetComponent<AssemblyRoomRunner>();
        await UniTask.WaitUntil(() => room.StateMachine.State == AssemblyRoomRunner.GameStates.Gaming);
        SetAssimblyRoom(impRoom.GetComponent<IAssemblyRoom>());

        // ==== comment shopDispatcher ====
        // shopDispatcher.setAbilityAction += room.AbilityManager.SetAbilityToEntry;
        // shopDispatcher.setNullAbilityAction += room.AbilityManager.SetAbilityOutOfEntry;
        // shopDispatcher.refreshAbilityAction += RefreshAbillity;
        // shopDispatcher.refreshNullAbilityAction += RefreshNullAbillity;
        // shopDispatcher.rebindKeyAction += room.AbilityRebinder.StartRebinding;
        // 
        // shopDispatcher.RefreshAllBoxAbility();
        // RefreshAllSkillBoxDisplayText();
    }

    /// <summary>
    /// Set AssimblyRoom for UI which can interact with it.
    /// </summary>
    /// <param name="Iar">new IAssemblyRoom.</param>
    public void SetAssimblyRoom(IAssemblyRoom Iar) {
        if(Iar == null) { Debug.Log("IAssemblyRoom is null."); return; }

        if(room != null) {
            // ==== comment shopDispatcher ====
            // room.AbilityRebinder.OnFinishRebinding -= shopDispatcher.SetRebindKeyText;
            // room.ControlledDevice.OnDeviceConnectionChanged -= shopDispatcher.RefreshAllBoxAbility;
            // room.assemblyController.OnGameComponentDraggedStart -= RefreshAllBoxAbilityAction;
            // room.assemblyController.OnGameComponentDraggedEnd -= RefreshAllBoxAbilityAction;
            // room.assemblyController.AfterGameComponentConnected -= RefreshAllBoxAbilityAction;
            room.assemblyController.OnGameComponentSelected -= UpdateUserCostRemain;
            room.assemblyController.AfterGameComponentConnected -= UpdateUserCostRemain;

            // ==== comment shopDispatcher ====
            // room.OnLoadedDevice -= shopDispatcher.RefreshAllBoxAbility;
            // room.AbilityManager.OnSetBinding += RefreshSkillBoxDisplayText;

            // fileCtrl.RemoveRenameAction(room.RenameDevice);
            // fileCtrl.StoreAction -= room.SaveCurrentDevice;
            // fileCtrl.LoadAction -= room.LoadDevice;
            fileCtrl.LoadAction -= room.LoadDevice;
        }

        room = Iar;
        // ==== comment shopDispatcher ====
        // GameEvents.RebindEvents.OnFinishRebinding += (_, str) => shopDispatcher.SetRebindKeyText(str);
        //room.AbilityRebinder.OnFinishRebinding += shopDispatcher.SetRebindKeyText;
        // ==== comment shopDispatcher ====
        // room.ControlledDevice.OnDeviceConnectionChanged += shopDispatcher.RefreshAllBoxAbility;
        // room.assemblyController.OnGameComponentDraggedStart += RefreshAllBoxAbilityAction;
        // room.assemblyController.OnGameComponentDraggedEnd += RefreshAllBoxAbilityAction;
        // room.assemblyController.AfterGameComponentConnected += RefreshAllBoxAbilityAction;
        room.assemblyController.OnGameComponentSelected += UpdateUserCostRemain;
        room.assemblyController.AfterGameComponentConnected += UpdateUserCostRemain;

        // ==== comment shopDispatcher ====
        // room.OnLoadedDevice += shopDispatcher.RefreshAllBoxAbility;
        // GameEvents.AbilityManagerEvents.OnSetBinding += RefreshSkillBoxDisplayText;
        //room.AbilityManager.OnSetBinding += RefreshSkillBoxDisplayText;

        shopPage.SetElements(room.GetGameComponentDataListByTypeForShop(GameComponentType.Basic), GameComponentType.Basic);
        shopPage.SetElements(room.GetGameComponentDataListByTypeForShop(GameComponentType.Attack), GameComponentType.Attack);
        shopPage.SetElements(room.GetGameComponentDataListByTypeForShop(GameComponentType.Functional), GameComponentType.Functional);
        shopPage.SetElements(room.GetGameComponentDataListByTypeForShop(GameComponentType.Movement), GameComponentType.Movement);
        shopPage.SetShopElementClickAction(ElementClickAction);
        // fileCtrl.SetRenameAction(room.RenameDevice);
        // fileCtrl.StoreAction += room.SaveCurrentDevice;
        // fileCtrl.LoadAction += room.LoadDevice;
        fileCtrl.LoadAction += room.LoadDevice;
        // SetStoreFileNames(room.GetSavedDeviceList());

        UpdateUserCostRemain(null);
    }

    /// <summary>
    /// Invoke function for create a new component.
    /// </summary>
    /// <param name="gcd"></param>
    public void ElementClickAction(GameComponentData gcd) {
        Debug.Log("Create : " + gcd.DisplayName);
        room?.CreateNewGameComponent(gcd, Vector2.zero);// IDK position value.
    }

    /// <summary>
    /// Change room mode between play and combine mode.
    /// </summary>
    public void SwitchRoomMode() {
        roomMode = (roomMode == AssemblyRoomMode.PlayMode) ? AssemblyRoomMode.ConnectionMode : AssemblyRoomMode.PlayMode;
    }

    private enum AssemblyRoomMode {
        PlayMode,
        ConnectionMode
    }

    /// <summary>
    /// Set all file name to the file list whilch display in load file menu.
    /// </summary>
    /// <param name="fileNameList"></param>
    public void SetStoreFileNames(List<string> fileNameList) {
        if(fileNameList == null) { return; }
        int i = 0;
        for(; i < fileCtrl.fileElements.Count; ++i){
            if(fileNameList.Count >= i) { break; }
            fileCtrl.fileElements[i].SetFileName(fileNameList[i]);
        }
    }

    /// <summary>
    /// Refresh the specified entry abilitys to ability list in SkillDispatcher.
    /// </summary>
    /// <param name="boxId"></param>
    public void RefreshAbillity(int boxId) {
        if(room == null || room.AbilityManager == null || room.AbilityManager.AbilityInputEntries.Count <= boxId) { shopDispatcher.abilityList = null; return; }
        shopDispatcher.abilityList = room.AbilityManager.AbilityInputEntries[boxId].Abilities;
    }

    /// <summary>
    /// Refresh the non assigned entry abilitys to ability list in SkillDispatcher.
    /// </summary>
    public void RefreshNullAbillity() {
        shopDispatcher.abilityList = room.AbilityManager.GetAbilitiesOutOfEntry();
    }

    /// <summary>
    /// Refresh all entries' trigger key text to UI.
    /// </summary>
    public void RefreshAllSkillBoxDisplayText() {
        string keyText;
        for(int i = 0; i < room.AbilityManager.AbilityInputEntryNumber; ++i) {
            shopDispatcher.rebindBoxId = i;
            keyText = room.AbilityManager.AbilityInputEntries[i].InputPath;
            shopDispatcher.SetRebindKeyText(keyText == string.Empty ? "Non" : keyText);
        }
    }
    public void RefreshSkillBoxDisplayText(int boxid, string keyText) {
        // Debug.Log("Call Set key text.");
        shopDispatcher.rebindBoxId = boxid;
        shopDispatcher.SetRebindKeyText(keyText == string.Empty ? "Non" : keyText);
    }

    public void RefreshAllBoxAbilityAction(IGameComponent igc) {
        shopDispatcher.RefreshAllBoxAbility();
    }

    public void UpdateUserCostRemain(IGameComponent igc) {
        userDisplayMoney.SetPrice(room.GetPlayerRemainedMoney());
    }
}
