using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyUI : MonoBehaviour , ISellElementSubmitable {
    [SerializeField] PriceCtrl CostRemain;
    [SerializeField] SideBar Shop;
    [SerializeField] ComponentDescription descriptionBox;

    IAssemblyRoom room = null;
    List<GameComponentData>[] componentList { get; set; } = new List<GameComponentData>[Enum.GetValues(typeof(GameComponentType)).Length];

    public Action<int> Buy { get; set; }
    public Action<int> OpenDescription { get; set; }
    public Action<int> CloseDescription { get; set; }

    private void Awake() {
        Shop.Sells.ForEach(se => {
            se.EventSubmitter = this;
        });

        Shop.GetSells += GetSells;
        Buy += BuyComponent;
        OpenDescription += OpenDescriptionBox;
        CloseDescription += CloseDescriptionBox;
    }
    private async void Start() {
        GameObject impRoom = GameObject.Find("RoomManager");
        var room = impRoom.GetComponent<AssemblyRoomRunner>();
        await UniTask.WaitUntil(() => room.StateMachine.State == AssemblyRoomRunner.GameStates.Gaming);
        SetAssimblyRoom(room);
        Shop.UpdateSellElements();
        UpdateCostRemain(null);
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

    public void SetAssimblyRoom(IAssemblyRoom Iar) {
        // remove leasteners
        if(room != null) {
            room.assemblyController.OnGameComponentSelected -= UpdateCostRemain;
            room.assemblyController.AfterGameComponentConnected -= UpdateCostRemain;
        }
        if (Iar == null) { return; }
        room = Iar;

        // add leasteners
        room.assemblyController.OnGameComponentSelected += UpdateCostRemain;
        room.assemblyController.AfterGameComponentConnected += UpdateCostRemain;
        // set variables.
        componentList[(int)GameComponentType.Attack] = room.GetGameComponentDataListByTypeForShop(GameComponentType.Attack);
        componentList[(int)GameComponentType.Basic] = room.GetGameComponentDataListByTypeForShop(GameComponentType.Basic);
        componentList[(int)GameComponentType.Functional] = room.GetGameComponentDataListByTypeForShop(GameComponentType.Functional);
        componentList[(int)GameComponentType.Movement] = room.GetGameComponentDataListByTypeForShop(GameComponentType.Movement);
    }
    List<GameComponentData> GetSells(GameComponentType ID) {
        return componentList[(int)ID];
    }

    void UpdateCostRemain(IGameComponent igc) {
        if (room == null) { return; }
        CostRemain.SetPrice(room.GetPlayerRemainedMoney());
    }

    void BuyComponent(int elementID) {
        // room?.CreateNewGameComponent(gcd, Vector2.zero);// IDK position value.
        if (room == null) { return; }

        room.CreateNewGameComponent(componentList[(int)Shop.displayComponentType][elementID], Vector2.zero);
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
