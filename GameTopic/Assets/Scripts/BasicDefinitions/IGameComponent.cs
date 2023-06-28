using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;




// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    public Transform CoreTransform { get; }
    public bool IsInDevice { get; }
    public Guid? GlobalComponentID { get; set; }
    public int? LocalComponentID { get; set; }
    public int ComponentGUID { get; set; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    public void Connect(IGameComponent otherComponent, ConnectorInfo info);
    public void Disconnect();
    public GameComponentInfo DumpInfo();
}

public interface IConnector
{
    int connectorID { get; set; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info);

    void UnlinkToConnector();
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
