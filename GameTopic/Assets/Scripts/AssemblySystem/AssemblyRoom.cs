using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}

public class AssemblyRoom : MonoBehaviour
{
    public IDevice ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    AssemblySystemManager assemblySystemManager;

    public AssemblyRoomMode Mode {get; private set;} = AssemblyRoomMode.ConnectionMode;

    private void Awake() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        assemblySystemManager = gameObject.AddComponent<AssemblySystemManager>();

        ControlledDevice = createSimpleDevice();
    }

    public void SetMode(AssemblyRoomMode mode){
        Mode = mode;
        
    }


    private IDevice createSimpleDevice(){
        var device = new GameObject("Device").AddComponent<Device>();
        device.GameComponentFactory = GameComponentFactory;
        var initComponent = GameComponentFactory.CreateGameComponentObject(0);
        device.RootGameComponent = initComponent;
        return device;
    }
}
