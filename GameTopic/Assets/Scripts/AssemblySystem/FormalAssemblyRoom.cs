using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameframe.SaveLoad;
using System.Linq;
using System;

public class FormalAssemblyRoom : MonoBehaviour, IAssemblyRoom
{

    #region Events
    /// <summary>
    /// Event for when the room mode is changed.
    /// </summary>
    public event Action<AssemblyRoomMode> OnSetRoomMode;

    public event Action OnLoadedDevice;
    public event Action OnSavedDevice;

    #endregion
    
    #region Properties

    public int PlayerInitMoney { get; set; } = 1000;

    /// <summary>
    /// The device that the FormalAssemblyRoom is controlling.
    /// </summary>
    public Device ControlledDevice { get; private set;}



    /// <summary>
    /// The manager for assembly system.
    /// </summary>
    public AssemblySystemManager AssemblySystemManager { get; private set;}
    /// <summary>
    /// The manager for recording game component units.
    /// </summary>
    /// <value></value>
    public UnitManager GameComponentsUnitManager { get; private set;}


    public AbilityManager AbilityManager => ControlledDevice.AbilityManager;

    /// <summary>
    /// The factory for creating game components.
    /// </summary>
    private IGameComponentFactory GameComponentFactory;

    public IAbilityKeyChanger AbilityKeyChanger { get; private set; }

    public List<GameComponentData> GameComponentDataList { get; private set; }

    public AbilityRunner AbilityRunner { get; private set; }

    public int CurrentLoadedDeviceID { get; private set; } = 0;

    #endregion

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        AssemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();
        AssemblySystemManager.GameComponentsUnitManager = GameComponentsUnitManager;

        ControlledDevice = createDevice();
        LoadDevice(CurrentLoadedDeviceID);


        AbilityKeyChanger = new AbilityKeyChanger(AbilityManager);
        AssemblySystemManager.OnGameComponentDraggedStart += (component) => {
            AbilityManager.UpdateDeviceAbilities();
            SaveCurrentDevice();
        };
        AssemblySystemManager.AfterGameComponentConnected += (component) => {
            AbilityManager.UpdateDeviceAbilities();
            SaveCurrentDevice();
        };

        AbilityKeyChanger.OnFinishChangeAbilityKey += _ => {
            AbilityManager.UpdateDeviceAbilities();
            Debug.Log("Ability key changed");
            SaveCurrentDevice();
        };

        GameComponentDataList = getGameComponentDataListFromResources();
        Debug.Assert(GameComponentDataList != null);

        Debug.Assert(PlayerInitMoney >= 0);

        AbilityRunner = gameObject.AddComponent<AbilityRunner>();
        AbilityRunner.AbilityManager = AbilityManager;

        
    }
    private void Start() {
        
        SetRoomMode(AssemblyRoomMode.PlayMode);
    }

    private Device createDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        device.GameComponentFactory = GameComponentFactory;
        return device;
    }

    private List<GameComponentData> getGameComponentDataListFromResources() {
        var dataList = ResourceManager.Instance.LoadAllGameComponentData();
        Debug.Assert(dataList != null);
        return dataList;
    
    }

    /// <summary>
    /// Get the total cost of all game components from the device root.
    /// </summary>
    /// <returns></returns>
    public int GetDeviceTotalCost() {
        var cost = 0;
        ControlledDevice.ForEachGameComponent((component) => {
            var data = GameComponentDataList.Where((data) => data.ResourcePath == component.ComponentName);
            Debug.Assert(data.Count() == 1, "GameComponentDataList should have data for data name: " + component.ComponentName + " but it doesn't.");
            cost += data.First().Price;
        });
        return cost;
    
    }

    private void clearAllGameComponents()
    {
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var component = unit as IGameComponent;
            Destroy(component.BodyTransform.gameObject);
        });
        GameComponentsUnitManager.Clear();
    }

    private IDevice LoadNewDevice(DeviceInfo deviceInfo)
    {
        deviceInfo ??= ResourceManager.Instance.LoadDefaultDeviceInfo();
        clearAllGameComponents();
        ControlledDevice.Load(deviceInfo);
        ControlledDevice.ForEachGameComponent((component) => {
            GameComponentsUnitManager.AddUnit(component);
        });
        OnLoadedDevice?.Invoke();
        return ControlledDevice;
    }

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        var path = componentData.ResourcePath;
        var newComponent = GameComponentFactory.CreateGameComponentObject(path);
        GameComponentsUnitManager.AddUnit(newComponent);
        newComponent.DragableTransform.position = position;
        return newComponent;
    }

    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type)
    {
        var filteredList = GameComponentDataList.Where((data) => data.Type == type && data.DisplayAtShop == true).ToList();
        return filteredList;
    }
    
    #region Save and Load Implementation

    public void SaveCurrentDevice(){
        var info = ControlledDevice.Dump();
        var deviceInfo = info as DeviceInfo;
        Debug.Assert(deviceInfo != null);
        ResourceManager.Instance.SaveLocalDeviceInfo(deviceInfo, CurrentLoadedDeviceID.ToString());
        OnSavedDevice?.Invoke();
    }
    public void LoadDevice(int DeviceID){
        CurrentLoadedDeviceID = DeviceID;
        var deviceInfo = ResourceManager.Instance.LoadLocalDeviceInfo(DeviceID.ToString());
        LoadNewDevice(deviceInfo);
    }
    #endregion

    public int GetPlayerRemainedMoney() {
        return PlayerInitMoney - GetDeviceTotalCost();
    }
    public void SetRoomMode(AssemblyRoomMode mode)
    {
        switch(mode){
            case AssemblyRoomMode.ConnectionMode:
                AssemblySystemManager.EnableAssemblyComponents();
                break;
            case AssemblyRoomMode.PlayMode:
                AssemblySystemManager.DisableAssemblyComponents();
                break;
        }
        OnSetRoomMode?.Invoke(mode);
    }
}

public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}
