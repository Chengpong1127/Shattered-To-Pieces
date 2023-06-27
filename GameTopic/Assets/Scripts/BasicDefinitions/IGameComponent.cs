using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// dump or load info for IConnector
public struct ConnectorInfo
{
    public int connectorID;
    public int linkedConnectorID;
    public int linkedTargetID;
    public float connectorRotation;
    public bool IsConnected => linkedConnectorID != -1;
    public static ConnectorInfo NoConnection(int connectorID){
        return new ConnectorInfo{
            connectorID = connectorID,
            linkedConnectorID = -1,
            linkedTargetID = -1,
            connectorRotation = 0f
        };
    }
}


// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    bool IsInDevice { get; }
    int? LocalComponentID { get; set;}
    int ComponentGUID { get; set; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    void Connect(IGameComponent otherComponent, ConnectorInfo info);
    void Disconnect();
    GameComponentInfo DumpInfo();
}

public interface IConnector
{
    int connectorID { get; set; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info);
    void Disconnect();
    ConnectorInfo Dump();
    (IConnector, int) GetAvailableConnector(); // return the first available connector and target ID
}

public interface ICoreComponent
{
    Dictionary<string, Ability> AllAbilities { get; }
}

public struct Ability{
    public string name;
    public UnityAction action;
    public Ability(string name, UnityAction action){
        this.name = name;
        this.action = action;
    }
}
