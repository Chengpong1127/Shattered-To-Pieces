using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameComponent : MonoBehaviour, IGameComponent, IUnit
{
    public int UnitID { get; set; }
    private IConnector connector;
    private ICoreComponent coreComponent;

    public IConnector Connector => connector;
    public ICoreComponent CoreComponent => coreComponent;
    public bool IsInDevice => false;
    public Transform CoreTransform { get => transform; }
    public int ComponentGUID { get; set; }

    public void Connect(IGameComponent otherComponent, ConnectionInfo info)
    {
        Debug.Assert(otherComponent != null);
        Debug.Assert(connector != null);
        Debug.Assert(otherComponent.Connector != null);
        connector.ConnectToComponent(otherComponent.Connector, info);
        
    }

    public void Disconnect()
    {
        connector.Disconnect();
    }

    public GameComponentInfo DumpInfo(){
        var info = new GameComponentInfo();
        info.componentGUID = ComponentGUID;
        info.connectorInfo = connector.Dump();
        return info;
    }

    public (IGameComponent, ConnectionInfo) GetAvailableConnection(){
        var (connectorPoint, targetID) = connector.GetAvailableConnector();
        Debug.Log($"Available connector: {connectorPoint}, {targetID}");
        if (connectorPoint == null){
            return (null, ConnectionInfo.NoConnection());
        }
        var gameComponent = (connectorPoint as MonoBehaviour).GetComponentInParent<GameComponent>();
        Debug.Assert(gameComponent != null);
        var newInfo = new ConnectionInfo{
            linkedTargetID = targetID,
        };
        return (gameComponent, newInfo);
    }

    private void Awake()
    {
        connector = GetComponentInChildren<Connector>();
        Debug.Assert(connector != null, "Connector not found");
        coreComponent = null;
    }
}
