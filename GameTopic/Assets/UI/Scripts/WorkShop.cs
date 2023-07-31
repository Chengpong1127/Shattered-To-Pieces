using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        // roomMode = AssemblyRoomMode.PlayMode;
        shoppingBTN.onClick.AddListener(SwitchRoomMode);
    }

    private void Start() {
        GameObject impRoom = GameObject.Find("RoomManager");
        SetAssimblyRoom(impRoom.GetComponent<IAssemblyRoom>());

        shopDispatcher.setAbilityAction += room.AbilityManager.SetAbilityToEntry;
        shopDispatcher.setNullAbilityAction += room.AbilityManager.SetAbilityOutOfEntry;
        shopDispatcher.refreshAbilityAction += RefreshAbillity;
        shopDispatcher.refreshNullAbilityAction += RefreshNullAbillity;
        shopDispatcher.rebindKeyAction += room.AbilityKeyChanger.StartChangeAbilityKey;

        shopDispatcher.RefreshAllBoxAbility();
        RefreshAllSkillBoxDisplayText();
    }

    /// <summary>
    /// Set AssimblyRoom for UI which can interact with it.
    /// </summary>
    /// <param name="Iar">new IAssemblyRoom.</param>
    public void SetAssimblyRoom(IAssemblyRoom Iar) {
        if(Iar == null) { Debug.Log("IAssemblyRoom is null."); return; }

        if(room != null) {
            room.AbilityKeyChanger.OnFinishChangeAbilityKey -= shopDispatcher.SetRebindKeyText;
            room.AssemblySystemManager.OnGameComponentDraggedStart -= RefreshAllBoxAbilityAction;
            room.AssemblySystemManager.OnGameComponentDraggedEnd -= RefreshAllBoxAbilityAction;
            room.AssemblySystemManager.AfterGameComponentConnected -= RefreshAllBoxAbilityAction;
            room.AssemblySystemManager.OnGameComponentDraggedStart -= UpdateUserCostRemain;
            room.AssemblySystemManager.AfterGameComponentConnected -= UpdateUserCostRemain;

            // fileCtrl.RemoveRenameAction(room.RenameDevice);
            // fileCtrl.StoreAction -= room.SaveCurrentDevice;
            // fileCtrl.LoadAction -= room.LoadDevice;
            fileCtrl.LoadAction -= room.LoadDevice;
        }

        room = Iar;

        room.AbilityKeyChanger.OnFinishChangeAbilityKey += shopDispatcher.SetRebindKeyText;
        room.AssemblySystemManager.OnGameComponentDraggedStart += RefreshAllBoxAbilityAction;
        room.AssemblySystemManager.OnGameComponentDraggedEnd += RefreshAllBoxAbilityAction;
        room.AssemblySystemManager.AfterGameComponentConnected += RefreshAllBoxAbilityAction;
        room.AssemblySystemManager.OnGameComponentDraggedStart += UpdateUserCostRemain;
        room.AssemblySystemManager.AfterGameComponentConnected += UpdateUserCostRemain;

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
        room.SetRoomMode(roomMode);
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
        room?.SetRoomMode(roomMode);
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

    public void RefreshAllBoxAbilityAction(IGameComponent igc) {
        shopDispatcher.RefreshAllBoxAbility();
    }

    public void UpdateUserCostRemain(IGameComponent igc) {
        userDisplayMoney.SetPrice(room.GetPlayerRemainedMoney());
    }
}
