using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    int ComponentID { get; set;}
    int ComponentGUID { get; set; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    void Connect(IGameComponent otherComponent, int targetID);
}

public interface IConnector
{
    int connectorID { get; set; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connecterPoint, int targetID);
}

public interface ICoreComponent
{

}