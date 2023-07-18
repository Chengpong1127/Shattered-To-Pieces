using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameframe.SaveLoad;

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
    private UnitManager GameComponentsUnitManager;
    /// <summary>
    /// The manager for device storage.
    /// </summary>
    private SaveLoadManager deviceStorageManager;

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        assemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();
        deviceStorageManager = SaveLoadManager.Create("BaseDirectory","SavedDevice",SerializationMethodType.JsonDotNet);
    }

    private void clearAllGameComponents()
    {
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var component = unit as IGameComponent;
            Destroy(component.CoreTransform);
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
        newComponent.CoreTransform.position = position;
    }

    public List<GameComponentData> GetGameComponentDataList(GameComponentType type)
    {
        throw new System.NotImplementedException();
    }

    public List<string> GetSavedDeviceList()
    {
        var files = deviceStorageManager.GetFiles();
        var deviceList = new List<string>();
        foreach(var file in files)
        {
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
        ControlledDevice.RootGameComponent.CoreTransform.position = position;
    }

    public void RenameDevice(string DeviceName, string NewDeviceName)
    {
        deviceStorageManager.Load<string>(DeviceName);
        deviceStorageManager.Save(NewDeviceName, DeviceName);
        deviceStorageManager.DeleteSave(DeviceName);
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
        throw new System.NotImplementedException();
    }
}
