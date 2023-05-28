using System.Collections.Generic;
using UnityEngine;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    int ComponentID { get; set;}
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    void Connect(IGameComponent otherComponent, int targetID);
}

public interface ITarget
{
    int targetID { get; set; }
    IConnector ownerIConnector { get; set; }

    void LinkTarget(IConnector lic);
    void UnLinkTarget(IConnector ulic);
    void ActiveITarget(bool active);
}

public interface IConnector
{
    int connectorID { get; set; }
    ITarget GetTargetByIndex(int targetID);
    void ConnectToComponent(IConnector connecterPoint, int targetID);
}

public interface ICoreComponent
{

}