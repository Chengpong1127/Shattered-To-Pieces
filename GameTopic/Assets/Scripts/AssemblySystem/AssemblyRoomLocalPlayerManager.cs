


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager, IAssemblyRoom{
    public AssemblyController assemblyController => Player != null ? Player.AssemblyController : null;

    public int PlayerInitMoney { get; set; } = 1000;

    public AbilityManager AbilityManager => Player.SelfDevice.AbilityManager;

    public IAbilityRebinder AbilityRebinder { get; private set; }

    public event Action<AssemblyRoomMode> OnSetRoomMode;
    public event Action OnLoadedDevice;
    public event Action OnSavedDevice;
    public int CurrentDeviceID { get; private set; } = 0;
    public List<GameComponentData> GameComponentDataList {
        get {
            _gameComponentDataList ??= ResourceManager.Instance.LoadAllGameComponentData();
            return _gameComponentDataList;
        }
    }
    private List<GameComponentData> _gameComponentDataList;
    private IGameComponentFactory _gameComponentFactory;

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        return _gameComponentFactory.CreateGameComponentObject(componentData.ResourcePath);
    }

    public int GetDeviceTotalCost() {
        if (Player.SelfDevice == null) return 0;
        var cost = 0;
        Player.SelfDevice.ForEachGameComponent((component) => {
            var data = GameComponentDataList.Where((data) => data.ResourcePath == component.ComponentName);
            Debug.Assert(data.Count() == 1, "GameComponentDataList should have data for data name: " + component.ComponentName + " but it doesn't.");
            cost += data.First().Price;
        });
        return cost;
    
    }

    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type)
    {
        var filteredList = GameComponentDataList.Where((data) => data.Type == type && data.DisplayAtShop == true).ToList();
        return filteredList;
    }

    public int GetPlayerRemainedMoney()
    {
        return PlayerInitMoney - GetDeviceTotalCost();
    }

    public void LoadDevice(int DeviceID)
    {
        SaveCurrentDevice();
        CleanAllGameComponents();
        Player.LoadLocalDevice(DeviceID.ToString());
        Player.LocalAbilityActionMap.Enable();
        CurrentDeviceID = DeviceID;
        AbilityRebinder = new AbilityRebinder(AbilityManager, Player.LocalAbilityActionMap);
        OnLoadedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnLoadedDevice.Invoke();
    }
    private void UpdateAbility(){
        Player.SelfDevice.AbilityManager.UpdateDeviceAbilities();
    }

    public void SaveCurrentDevice()
    {
        if (Player.SelfDevice == null) return;
        var info = Player.SelfDevice.Dump();
        ResourceManager.Instance.SaveLocalDeviceInfo(info as DeviceInfo, CurrentDeviceID.ToString());
        OnSavedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnSavedDevice.Invoke();
    }

    public void SetRoomMode(AssemblyRoomMode mode)
    {
        switch(mode){
            case AssemblyRoomMode.ConnectionMode:
                break;
            case AssemblyRoomMode.PlayMode:
                break;
        }
        OnSetRoomMode?.Invoke(mode);
    }
    private void CleanAllGameComponents(){
        Player.SelfDevice.ForEachGameComponent((component) => {
            component.Die();
        });
        NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
            .Select((obj) => obj.GetComponentInParent<IGameComponent>())
            .Where((component) => component != null)
            .ToList()
            .ForEach((component) => {
                component.Die();
            });
    }
    private void OnApplicationQuit() {
        SaveCurrentDevice();
    }

    protected override void PlayerSetup(){
        Player.LocalAbilityActionMap.Enable();
        _gameComponentFactory = new NetworkGameComponentFactory();
        LoadDevice(CurrentDeviceID);
        assemblyController.OnGameComponentDraggedStart += _ => UpdateAbility();
        assemblyController.AfterGameComponentConnected += _ => UpdateAbility();
    }
}