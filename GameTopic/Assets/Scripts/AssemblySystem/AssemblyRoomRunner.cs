using UnityEngine;
using Unity.Netcode;
using System.Linq;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class AssemblyRoomRunner: BaseGameRunner, IAssemblyRoom{
    public GamePlayer ControlledPlayer => PlayerMap.Values.First() as GamePlayer;
    public Device ControlledDevice => ControlledPlayer.SelfDevice;

    public AssemblyController assemblyController => ControlledPlayer.AssemblyController;

    public int PlayerInitMoney { get; set; } = 1000;

    public AbilityManager AbilityManager => ControlledDevice.AbilityManager;

    public IAbilityRebinder AbilityRebinder => ControlledPlayer.AbilityRebinder;
    public int GetPlayerRemainedMoney()
    {
        return PlayerInitMoney - GetDeviceTotalCost();
    }
    public event Action OnLoadedDevice;
    public event Action OnSavedDevice;
    private IGameComponentFactory _gameComponentFactory;
    public List<GameComponentData> GameComponentDataList {
        get {
            _gameComponentDataList ??= ResourceManager.Instance.LoadAllGameComponentData();
            return _gameComponentDataList;
        }
    }
    private List<GameComponentData> _gameComponentDataList;
    public int GetDeviceTotalCost() {
        if (ControlledPlayer.SelfDevice == null) return 0;
        var cost = 0;
        ControlledPlayer.SelfDevice.ForEachGameComponent((component) => {
            var data = GameComponentDataList.Where((data) => data.ResourcePath == component.ComponentName);
            Debug.Assert(data.Count() == 1, "GameComponentDataList should have data for data name: " + component.ComponentName + " but it doesn't.");
            cost += data.First().Price;
        });
        return cost;
    
    }
    public void SaveCurrentDevice()
    {
        if (ControlledPlayer.SelfDevice == null) return;
        var info = ControlledPlayer.SelfDevice.Dump();
        ResourceManager.Instance.SaveLocalDeviceInfo(info as DeviceInfo, CurrentDeviceID.ToString());
        OnSavedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnSavedDevice.Invoke();
    }

    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type)
    {
        var filteredList = GameComponentDataList.Where((data) => data.Type == type && data.DisplayAtShop == true).ToList();
        return filteredList;
    }
    public int CurrentDeviceID { get; private set; } = 0;

    private void Initializing_Enter(){
        _gameComponentFactory = new NetworkGameComponentFactory();
    }


    protected override void GameStartSpawnAllPlayer()
    {
        LoadDevice(CurrentDeviceID);
    }

    public void CleanAllGameComponents(){
        PlayerMap.Values.ToList().ForEach(player => {
            if(player.IsAlive.Value)
                player?.SelfDevice.ForEachGameComponent((component) => {
                    component.Die();
                }
            );
        });
        NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
            .Select((obj) => obj.GetComponentInParent<BaseEntity>())
            .Where((entity) => entity != null)
            .Where(entity => entity.NetworkObject.IsSceneObject == false)
            .ToList()
            .ForEach((component) => {
                component.Die();
        });
    }

    public override void SpawnDevice(BasePlayer player, string filename){
        CleanAllGameComponents();
        player.ServerLoadDevice(filename, Vector3.zero);
    }

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        return _gameComponentFactory.CreateGameComponentObject(componentData.ResourcePath, Vector3.zero);
    }

    public void LoadDevice(int DeviceID)
    {
        SaveCurrentDevice();
        SpawnDevice(ControlledPlayer, DeviceID.ToString());
        ControlledPlayer.LocalAbilityActionMap.Enable();
        CurrentDeviceID = DeviceID;
        OnLoadedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnLoadedDevice.Invoke();
    }
    private void OnApplicationQuit() {
        SaveCurrentDevice();
    }
}