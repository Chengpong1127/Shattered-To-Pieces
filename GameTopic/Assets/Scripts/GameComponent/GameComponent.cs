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

    public void Connect(IGameComponent otherComponent, ConnectorInfo info)
    {
        Debug.Assert(otherComponent != null);
        Debug.Assert(connector != null);
        Debug.Assert(otherComponent.Connector != null);
        connector.ConnectToComponent(otherComponent.Connector, info);
        
    }

    public GameComponentInfo DumpInfo(){
        var info = new GameComponentInfo();
        info.componentGUID = ComponentGUID;
        //info.connectorInfo = connector.Dump();
        return info;
    }

    private void Awake()
    {
        connector = GetComponentInChildren<Connector>();
        Debug.Assert(connector != null, "Connector not found");
        coreComponent = null;
    }
}
