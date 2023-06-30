using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;




// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    public Transform CoreTransform { get; }
    public bool IsInDevice { get; }
    public int ComponentGUID { get; set; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    public void ConnectToParent(IGameComponent otherComponent, ConnectionInfo info);
    public void DisconnectFromParent();
    public (IGameComponent, ConnectionInfo) GetAvailableConnection();
    public GameComponentInfo DumpInfo();
}

public interface IConnector
{
    public IGameComponent GameComponent { get; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info);

    void UnlinkToConnector();
    void Disconnect();
    public void SetConnectMode(bool connectMode);
    ConnectionInfo Dump();
    (IConnector, int) GetAvailableConnector(); // return the first available connector and target ID

    public ICollection<IConnector> GetChildConnectors();
    public IConnector GetParentConnector();
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
