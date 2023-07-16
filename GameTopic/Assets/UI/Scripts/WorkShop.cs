using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WorkShop : MonoBehaviour
{
    IAssemblyRoom room;
    AssemblyRoomMode roomMode;

    [SerializeField] PriceCtrl userDisplayMoney;
    [SerializeField] Button shoppingBTN;
    [SerializeField] Button settingBTN;
    [SerializeField] Button storeBTN;
    [SerializeField] Button loladBTN;
    [SerializeField] Button exitBTN;
    [SerializeField] ShopBGCtrl shopPage;
    [SerializeField] StoreFileCtrl fileCtrl;


    private void Awake() {
        roomMode = AssemblyRoomMode.PlayMode;
        shoppingBTN.onClick.AddListener(SwitchRoomMode);

        // delete after implement IAssemblyRooms
        shopPage.SetShopElementClickAction(ElementClickAction);
        fileCtrl.SetRenameAction((string oldName, string newName) => {
            Debug.Log("Rename " + oldName + " to " + newName);
        });
        fileCtrl.StoreAction += (string fileName) => {
            Debug.Log("Get StoreFileName : " + fileName);
        };
        fileCtrl.LoadAction += (string fileName) => {
            Debug.Log("Get LoadFileName : " + fileName);
        };
    }


    public void SetAssimblyRoom(IAssemblyRoom Iar) {
        room = Iar;

        shopPage.SetElements(room.GetGameComponentDataList(GameComponentType.Basic), GameComponentType.Basic);
        shopPage.SetShopElementClickAction(ElementClickAction);
        fileCtrl.SetRenameAction(room.RenameDevice);
    }

    public void ElementClickAction(GameComponentData gcd) {
        Debug.Log("Create : " + gcd.DisplayName);
        room?.CreateNewGameComponent(gcd, Vector2.zero);
    }

    public void SwitchRoomMode() {
        roomMode = roomMode == AssemblyRoomMode.PlayMode ? AssemblyRoomMode.ConnectionMode : AssemblyRoomMode.PlayMode;
        // Debug.Log("switch to : " + roomMode.ToString());
        room?.SetRoomMode(roomMode);
    }
}
