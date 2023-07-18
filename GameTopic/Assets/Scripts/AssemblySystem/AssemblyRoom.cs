using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameframe.SaveLoad;
public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}

public class AssemblyRoom : MonoBehaviour, IAssemblyRoom
{
    public Device ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    public AssemblySystemManager assemblySystemManager;
    UnitManager GameComponentsUnitManager;

    public TempAbilityInputUI tempAbilityInputUI;
    public TempAbilityInputUI idleTempAbilityInputUI;
    public AbilityInputManager abilityInputManager;
    

    public AssemblyRoomMode Mode {get; private set;} = AssemblyRoomMode.ConnectionMode;

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        assemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();
        abilityInputManager = new AbilityInputManager();
        tempAbilityInputUI.abilityInputManager = abilityInputManager;


        assemblySystemManager.GameComponentsUnitManager = GameComponentsUnitManager;
        ControlledDevice = createSimpleDevice();
        GameComponentsUnitManager.AddUnit(ControlledDevice.RootGameComponent);
    }
    public void CreateNewComponent(int componentID){
        var newComponent = GameComponentFactory.CreateGameComponentObject(componentID);
        GameComponentsUnitManager.AddUnit(newComponent);

        newComponent.DragableTransform.position = new Vector3(0, 0, 0);
    }

    public void SetPlayMode(){
        Mode = AssemblyRoomMode.PlayMode;
        assemblySystemManager.DisableAssemblyComponents();

        idleTempAbilityInputUI.SetItems(ControlledDevice.getAbilityList());
    }

    public void SetConnectMode(){
        Mode = AssemblyRoomMode.ConnectionMode;
        assemblySystemManager.EnableAssemblyComponents();
    }

    private void LoadNewDevice(DeviceInfo deviceInfo){

        ClearAllGameComponents();
        ControlledDevice.Load(deviceInfo);
        ControlledDevice.ForEachGameComponent((component) => {
            GameComponentsUnitManager.AddUnit(component);
        });
    }

    private void ClearAllGameComponents(){
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var gameComponent = unit as GameComponent;
            if(gameComponent != null){
                Destroy(gameComponent.gameObject);
            }
        });
        GameComponentsUnitManager.Clear();
    }

    private Device createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        device.GameComponentFactory = GameComponentFactory;
        var initComponent = GameComponentFactory.CreateGameComponentObject(0);
        device.RootGameComponent = initComponent;
        return device;
    }

    void IAssemblyRoom.CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    void IAssemblyRoom.SetRoomMode(AssemblyRoomMode mode)
    {
        throw new System.NotImplementedException();
    }

    List<GameComponentData> IAssemblyRoom.GetGameComponentDataList(GameComponentType type)
    {
        throw new System.NotImplementedException();
    }

    public void SaveCurrentDevice(string deviceName)
    {
        var info = ControlledDevice.Dump();
        var deviceInfo = info as DeviceInfo;
        Debug.Assert(deviceInfo != null);
        var filename = deviceName + ".json";
        manager.Save(deviceInfo, filename);
    }

    public void LoadDevice(string deviceName)
    {
        var filename = deviceName + ".json";
        var deviceInfo = manager.Load<DeviceInfo>(filename);
        LoadNewDevice(deviceInfo);
    }

    void IAssemblyRoom.LoadDevice(string DeviceName, Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    void IAssemblyRoom.RenameDevice(string DeviceName, string NewDeviceName)
    {
        throw new System.NotImplementedException();
    }

    List<string> IAssemblyRoom.GetSavedDeviceList()
    {
        throw new System.NotImplementedException();
    }
}
