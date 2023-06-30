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

    public void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info)
    {
        Debug.Assert(parentComponent != null);
        Debug.Assert(connector != null);
        Debug.Assert(parentComponent.Connector != null);
        connector.ConnectToComponent(parentComponent.Connector, info);
        
    }

    public void DisconnectFromParent()
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
        var (availableParent, targetID) = connector.GetAvailableConnector();
        Debug.Log($"Available connector: {availableParent}, {targetID}");
        if (availableParent == null){
            return (null, ConnectionInfo.NoConnection());
        }
        var gameComponent = (availableParent as MonoBehaviour).GetComponentInParent<GameComponent>();
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
