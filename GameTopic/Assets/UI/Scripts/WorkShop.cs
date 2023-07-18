using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WorkShop : MonoBehaviour
{
    AssemblyRoomMode roomMode;

    [SerializeField] IAssemblyRoom room;
    [SerializeField] PriceCtrl userDisplayMoney;
    [SerializeField] Button shoppingBTN;
    [SerializeField] Button settingBTN;
    [SerializeField] Button storeBTN;
    [SerializeField] Button loladBTN;
    [SerializeField] Button exitBTN;
    [SerializeField] ShopBGCtrl shopPage;
    [SerializeField] StoreFileCtrl fileCtrl;


    private void Awake() {
        // delete after implement IAssemblyRooms
        // shopPage.SetShopElementClickAction(ElementClickAction);
        // fileCtrl.SetRenameAction((string oldName, string newName) => {
        //     Debug.Log("Rename " + oldName + " to " + newName);
        // });
        // fileCtrl.StoreAction += (string fileName) => {
        //     Debug.Log("Get StoreFileName : " + fileName);
        // };
        // fileCtrl.LoadAction += (string fileName) => {
        //     Debug.Log("Get LoadFileName : " + fileName);
        // };
    }

    private void Start() {
        roomMode = AssemblyRoomMode.PlayMode;
        shoppingBTN.onClick.AddListener(SwitchRoomMode);


        GameObject impRoom = GameObject.Find("RoomManager");
        SetAssimblyRoom(impRoom.GetComponent<IAssemblyRoom>());
    }

    /// <summary>
    /// Set AssimblyRoom for UI which can interact with it.
    /// </summary>
    /// <param name="Iar">new IAssemblyRoom.</param>
    public void SetAssimblyRoom(IAssemblyRoom Iar) {
        if(Iar == null) { Debug.Log("IAssemblyRoom is null."); return; }

        if(room != null) {
            fileCtrl.RemoveRenameAction(room.RenameDevice);
            fileCtrl.StoreAction -= room.SaveCurrentDevice;
            fileCtrl.LoadAction -= room.LoadDevice;
        }

        room = Iar;

        shopPage.SetElements(room.GetGameComponentDataList(GameComponentType.Basic), GameComponentType.Basic);
        shopPage.SetShopElementClickAction(ElementClickAction);
        fileCtrl.SetRenameAction(room.RenameDevice);
        fileCtrl.StoreAction += room.SaveCurrentDevice;
        fileCtrl.LoadAction += room.LoadDevice;
        SetStoreFileNames(room.GetSavedDeviceList());
    }

    /// <summary>
    /// Invoke function for create a new component.
    /// Depends on AssemblyRoom.
    /// </summary>
    /// <param name="gcd"></param>
    public void ElementClickAction(GameComponentData gcd) {
        Debug.Log("Create : " + gcd.DisplayName);
        room?.CreateNewGameComponent(gcd, Vector2.zero);// IDK position value.
    }

    /// <summary>
    /// Change room mode between play and combine mode.
    /// Depends on AssemblyRoom.
    /// </summary>
    public void SwitchRoomMode() {
        roomMode = roomMode == AssemblyRoomMode.PlayMode ? AssemblyRoomMode.ConnectionMode : AssemblyRoomMode.PlayMode;
        room?.SetRoomMode(roomMode);
    }
    public void SetStoreFileNames(List<string> fileNameList) {
        if(fileNameList == null) { return; }
        int i = 0;
        for(; i < fileCtrl.fileElements.Count; ++i){
            if(fileNameList.Count >= i) { break; }
            fileCtrl.fileElements[i].SetFileName(fileNameList[i]);
        }
    }
}
