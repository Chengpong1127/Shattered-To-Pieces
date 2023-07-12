using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}

public class AssemblyRoom : MonoBehaviour
{
    public Device ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    AssemblySystemManager assemblySystemManager;
    UnitManager GameComponentsUnitManager;

    public TempAbilityInputUI tempAbilityInputUI;
    

    public AssemblyRoomMode Mode {get; private set;} = AssemblyRoomMode.ConnectionMode;

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        assemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();
        GameComponentsUnitManager = new UnitManager();
        assemblySystemManager.tempAbilityInputUI = tempAbilityInputUI;

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
    }

    public void SetConnectMode(){
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
}
