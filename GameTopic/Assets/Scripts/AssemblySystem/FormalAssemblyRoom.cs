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

    public event Action<string> OnFinishChangeAbilityKey;

    #endregion
    
    #region Properties

    public int PlayerInitMoney { get; set; } = 1000;

    /// <summary>
    /// The device that the FormalAssemblyRoom is controlling.
    /// </summary>
    public Device ControlledDevice { get; private set;}

    public int GetPlayerRemainedMoney() {
        return PlayerInitMoney - GetDeviceTotalCost();
    }

    /// <summary>
    /// The manager for assembly system.
    /// </summary>
    public AssemblySystemManager AssemblySystemManager { get; private set;}
    /// <summary>
    /// The manager for recording game component units.
    /// </summary>
    /// <value></value>
    public UnitManager GameComponentsUnitManager { get; private set;}

    /// <summary>
    /// The manager for device storage.
    /// </summary>
    

    public AbilityManager AbilityManager { get; private set;}

    /// <summary>
    /// The factory for creating game components.
    /// </summary>
    private IGameComponentFactory GameComponentFactory;

    private SaveLoadManager deviceStorageManager;

    private IAbilityKeyChanger abilityKeyChanger;

    public List<GameComponentData> GameComponentDataList;

    

    #endregion

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        AssemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();
        AssemblySystemManager.GameComponentsUnitManager = GameComponentsUnitManager;
        deviceStorageManager = SaveLoadManager.Create("BaseDirectory", "SavedDevice", SerializationMethodType.JsonDotNet);

        ControlledDevice = createSimpleDevice();
        AbilityManager = new AbilityManager(ControlledDevice);
        AssemblySystemManager.OnGameComponentDraggedStart += (component) => {
            AbilityManager.UpdateDeviceAbilities();
        };
        AssemblySystemManager.AfterGameComponentConnected += (component) => {
            AbilityManager.UpdateDeviceAbilities();
        };

        abilityKeyChanger = new AbilityChanger(AbilityManager);
        abilityKeyChanger.OnFinishChangeAbilityKey += (key) => {
            OnFinishChangeAbilityKey?.Invoke(key);
        };

        GameComponentDataList = getGameComponentDataListFromResources();
        Debug.Assert(GameComponentDataList != null);

        Debug.Assert(PlayerInitMoney >= 0);
    }
    private void Start() {
        SetRoomMode(AssemblyRoomMode.PlayMode);
    }

    private Device createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        device.GameComponentFactory = GameComponentFactory;
        var initComponent = GameComponentFactory.CreateGameComponentObject("ControlRoom");
        GameComponentsUnitManager.AddUnit(initComponent);
        device.RootGameComponent = initComponent;
        return device;
    }

    private List<GameComponentData> getGameComponentDataListFromResources() {
        var dataList = ResourceManager.LoadAllGameComponentData();
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
            Destroy(component.DragableTransform.gameObject);
        });
        GameComponentsUnitManager.Clear();
    }

    private IDevice loadNewDevice(DeviceInfo deviceInfo)
    {
        clearAllGameComponents();
        ControlledDevice.Load(deviceInfo);
        ControlledDevice.ForEachGameComponent((component) => {
            GameComponentsUnitManager.AddUnit(component);
        });
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

    public List<string> GetSavedDeviceList()
    {
        var files = deviceStorageManager.GetFiles();
        var deviceList = new List<string>();
        foreach(var file in files)
        {
            if(file.EndsWith(".json"))
            deviceList.Add(file);
        }
        return deviceList;
    }

    public void LoadDevice(string DeviceName)
    {
        var filename = DeviceName + ".json";
        var deviceInfo = deviceStorageManager.Load<DeviceInfo>(filename);
        loadNewDevice(deviceInfo);
    }

    public void LoadDevice(string DeviceName, Vector2 position)
    {
        var filename = DeviceName + ".json";
        var deviceInfo = deviceStorageManager.Load<DeviceInfo>(filename);
        loadNewDevice(deviceInfo);
        ControlledDevice.RootGameComponent.DragableTransform.position = position;
    }

    public void RenameDevice(string DeviceName, string NewDeviceName)
    {
        var filename = DeviceName + ".json";
        var newFilename = NewDeviceName + ".json";
        var deviceInfo = deviceStorageManager.Load<DeviceInfo>(filename);
        deviceStorageManager.Save(deviceInfo, newFilename);
        deviceStorageManager.DeleteSave(filename);
    }

    public void SaveCurrentDevice(string DeviceName)
    {
        var info = ControlledDevice.Dump();
        var deviceInfo = info as DeviceInfo;
        Debug.Assert(deviceInfo != null);
        var filename = DeviceName + ".json";
        deviceStorageManager.Save(deviceInfo, filename);
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

    public void StartChangeAbilityKey(int abilityButtonID)
    {
        abilityKeyChanger.StartChangeAbilityKey(abilityButtonID);
    }

    public void EndChangeAbilityKey()
    {
        abilityKeyChanger.EndChangeAbilityKey();
    }
}
