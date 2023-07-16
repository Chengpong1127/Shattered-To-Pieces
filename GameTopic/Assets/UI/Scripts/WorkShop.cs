using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkShop : MonoBehaviour
{
    IAssemblyRoom room;
    bool isCombine;

    [SerializeField] PriceCtrl userDisplayMoney;
    [SerializeField] Button shoppingBTN;
    [SerializeField] Button settingBTN;
    [SerializeField] Button storeBTN;
    [SerializeField] Button loladBTN;
    [SerializeField] Button exitBTN;
    [SerializeField] ShopBGCtrl shopPage;
    [SerializeField] StoreFileCtrl fileCtrl;


    private void Awake() {
        isCombine = false;
        shopPage.SetShopElementClickAction(ElementClickAction);
    }


    public void SetAssimblyRoom(IAssemblyRoom Iar) {
        room = Iar;

        shopPage.SetElements(room.GetGameComponentDataList(GameComponentType.Basic), GameComponentType.Basic);
        shopPage.SetShopElementClickAction(ElementClickAction);
    }

    public void ElementClickAction(GameComponentData gcd) {
        Debug.Log("Create : " + gcd.DisplayName);
        room?.CreateNewGameComponent(gcd, Vector2.zero);
    }
}
