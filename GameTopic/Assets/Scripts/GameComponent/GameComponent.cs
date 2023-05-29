using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponent : MonoBehaviour, IGameComponent
{
    private int _gameComponentID;
    private IConnector connector;
    private ICoreComponent coreComponent;

    public IConnector Connector => connector;
    public ICoreComponent CoreComponent => coreComponent;


    public int ComponentID
    {
        get => _gameComponentID;
        set => _gameComponentID = value;
    }
    public int ComponentGUID { get; set; }

    public void Connect(IGameComponent otherComponent, int targetID)
    {
        Debug.Assert(otherComponent != null);
        Debug.Assert(connector != null);
        Debug.Assert(otherComponent.Connector != null);
        Debug.Log("Connect Component:" + ComponentID + " to Component:" + otherComponent.ComponentID + " at target:" + targetID);
        connector.ConnectToComponent(otherComponent.Connector, targetID);
        
    }

    public void DumpInfo(){

    }

    private void Awake()
    {
        connector = GetComponentInChildren<Connector>();
        Debug.Assert(connector != null, "Connector not found");
        coreComponent = null;
    }
}
