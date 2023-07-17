using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}

public class AssemblyRoom : MonoBehaviour, IAssemblyRoom
{
    public Device ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    AssemblySystemManager assemblySystemManager;
    UnitManager GameComponentsUnitManager;
    TempButtonGenerator tempButtonGenerator;

    public AssemblyRoomMode Mode {get; private set;} = AssemblyRoomMode.ConnectionMode;

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        assemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();


        assemblySystemManager.GameComponentsUnitManager = GameComponentsUnitManager;
        ControlledDevice = createSimpleDevice();
        GameComponentsUnitManager.AddUnit(ControlledDevice.RootGameComponent);
        tempButtonGenerator = gameObject.GetComponent<TempButtonGenerator>();
    }
    public void CreateNewComponent(int componentID){
        var newComponent = GameComponentFactory.CreateGameComponentObject(componentID);
        GameComponentsUnitManager.AddUnit(newComponent);

        newComponent.CoreTransform.position = new Vector3(0, 0, 0);
    }

    public void SetPlayMode(){
        Mode = AssemblyRoomMode.PlayMode;
        assemblySystemManager.DisableAssemblyComponents();
        tempButtonGenerator.GenerateButtons(ControlledDevice.getAbilityList());
    }

    public void SetConnectMode(){
        tempButtonGenerator.clearAllButtons();
        Mode = AssemblyRoomMode.ConnectionMode;
        assemblySystemManager.EnableAssemblyComponents();
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

    void IAssemblyRoom.SaveCurrentDevice(string DeviceName)
    {
        throw new System.NotImplementedException();
    }

    void IAssemblyRoom.LoadDevice(string DeviceName)
    {
        throw new System.NotImplementedException();
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
