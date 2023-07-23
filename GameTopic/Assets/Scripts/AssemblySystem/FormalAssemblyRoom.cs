using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameframe.SaveLoad;
using System.Linq;
using System;

public class FormalAssemblyRoom : MonoBehaviour, IAssemblyRoom
{
    /// <summary>
    /// The device that the FormalAssemblyRoom is controlling.
    /// </summary>
    public Device ControlledDevice;
    /// <summary>
    /// The factory for creating game components.
    /// </summary>
    private IGameComponentFactory GameComponentFactory;
    /// <summary>
    /// The manager for assembly system.
    /// </summary>
    private AssemblySystemManager assemblySystemManager;
    public UnitManager GameComponentsUnitManager;
    /// <summary>
    /// The manager for device storage.
    /// </summary>
    private SaveLoadManager deviceStorageManager;

    public AbilityManager AbilityManager { get; private set;}

    private IAbilityKeyChanger abilityKeyChanger;

    public event Action<string> OnFinishChangeAbilityKey;

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        assemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();
        assemblySystemManager.GameComponentsUnitManager = GameComponentsUnitManager;
        deviceStorageManager = SaveLoadManager.Create("BaseDirectory","SavedDevice",SerializationMethodType.JsonDotNet);

        ControlledDevice = createSimpleDevice();
        AbilityManager = new AbilityManager(ControlledDevice);
    }

    private Device createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        device.GameComponentFactory = GameComponentFactory;
        var initComponent = GameComponentFactory.CreateGameComponentObject("ControlRoom");
        GameComponentsUnitManager.AddUnit(initComponent);
        device.RootGameComponent = initComponent;
        return device;
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

    public void CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        var path = componentData.ResourcePath;
        var newComponent = GameComponentFactory.CreateGameComponentObject(path);
        GameComponentsUnitManager.AddUnit(newComponent);
        newComponent.DragableTransform.position = position;
    }

    public List<GameComponentData> GetGameComponentDataList(GameComponentType type)
    {
        var dataList = ResourceManager.LoadAllGameComponentData();
        Debug.Assert(dataList != null);
        var filteredList = dataList.Where((data) => data.Type == type).ToList();

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
                assemblySystemManager.EnableAssemblyComponents();
                break;
            case AssemblyRoomMode.PlayMode:
                assemblySystemManager.DisableAssemblyComponents();
                break;
        }
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
