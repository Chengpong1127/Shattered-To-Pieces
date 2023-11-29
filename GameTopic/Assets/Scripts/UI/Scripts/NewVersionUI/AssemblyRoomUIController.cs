using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyRoomUIController : MonoBehaviour , ISellElementSubmitable {
    [SerializeField] PriceCtrl CostRemain;
    [SerializeField] SideBar Shop;
    [SerializeField] ComponentDescription descriptionBox;
    [SerializeField] AssemblyRoomRunner assemblyRoomRunner;
    List<GameComponentData>[] componentList { get; set; } = new List<GameComponentData>[Enum.GetValues(typeof(GameComponentType)).Length];

    public Action<int> Buy { get; set; }
    public Action<int> OpenDescription { get; set; }
    public Action<int> CloseDescription { get; set; }

    private void Awake() {
        Debug.Assert(CostRemain != null);
        Debug.Assert(Shop != null);
        Debug.Assert(descriptionBox != null);
        Debug.Assert(assemblyRoomRunner != null);

        Shop.Sells.ForEach(se => {
            se.EventSubmitter = this;
        });

        Shop.GetSells += GetSells;
        Buy += BuyComponent;
        OpenDescription += OpenDescriptionBox;
        CloseDescription += CloseDescriptionBox;
        assemblyRoomRunner.OnMoneyChanged += MoneyChangedHandler;
    }
    private void MoneyChangedHandler(int money) {
        CostRemain.SetPrice(assemblyRoomRunner.GetPlayerRemainedMoney());
    }
    private async void Start() {
        await UniTask.WaitUntil(() => assemblyRoomRunner.StateMachine.State == AssemblyRoomRunner.GameStates.Gaming);
        SetAssimblyRoom(assemblyRoomRunner);
        Shop.UpdateSellElements();
        CostRemain.SetPrice(assemblyRoomRunner.GetPlayerRemainedMoney());
    }
    private void OnDestroy() {
        Shop.GetSells -= GetSells;
        OpenDescription -= OpenDescriptionBox;
        CloseDescription -= CloseDescriptionBox;
        Buy = null;
        OpenDescription = null;
        CloseDescription = null;
        SetAssimblyRoom(null);
    }

    public void SetAssimblyRoom(AssemblyRoomRunner Iar) {
        // remove leasteners
        if (Iar == null) { return; }
        assemblyRoomRunner = Iar;

        componentList[(int)GameComponentType.Attack] = assemblyRoomRunner.GetGameComponentDataListByTypeForShop(GameComponentType.Attack);
        componentList[(int)GameComponentType.Basic] = assemblyRoomRunner.GetGameComponentDataListByTypeForShop(GameComponentType.Basic);
        componentList[(int)GameComponentType.Functional] = assemblyRoomRunner.GetGameComponentDataListByTypeForShop(GameComponentType.Functional);
        componentList[(int)GameComponentType.Movement] = assemblyRoomRunner.GetGameComponentDataListByTypeForShop(GameComponentType.Movement);
    }
    List<GameComponentData> GetSells(GameComponentType ID) {
        return componentList[(int)ID];
    }

    void BuyComponent(int elementID) {
        // room?.CreateNewGameComponent(gcd, Vector2.zero);// IDK position value.
        if (assemblyRoomRunner == null) { return; }

        assemblyRoomRunner.CreateNewGameComponent(componentList[(int)Shop.displayComponentType][elementID], new Vector2(0.5f,0));
    }
    void OpenDescriptionBox(int elementID) {
        descriptionBox.SetDisplay(
            componentList[(int)Shop.displayComponentType][elementID].DisplayImage,
            componentList[(int)Shop.displayComponentType][elementID].DisplayName + " : " +
            componentList[(int)Shop.displayComponentType][elementID].Description);
        descriptionBox.gameObject.SetActive(true);
    }
    void CloseDescriptionBox(int elementID) {
        descriptionBox.gameObject.SetActive(false);
    }
}
