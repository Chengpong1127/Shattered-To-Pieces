using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// dump or load info for IConnector
public struct ConnectorInfo
{
    public int connectorID;
    public int linkedConnectorID;
    public int linkedTargetID;
    public Vector2 connectorRotation;
}


// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    int ComponentID { get; set;}
    int ComponentGUID { get; set; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    void Connect(IGameComponent otherComponent, ConnectorInfo info);
    GameComponentInfo DumpInfo();
}

public interface IConnector
{
    int connectorID { get; set; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connectorPoint, int targetID);

    void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info);
}

public interface ICoreComponent
{

}