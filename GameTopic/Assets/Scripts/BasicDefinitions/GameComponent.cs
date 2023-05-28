using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    Dictionary<ConnecterPoint, ConnecterPoint> ConnectorMap { get; }
}

public interface ITarget
{
    int targetID { get; set; }
    IConnector ownerIConnector { get; set; }
    void LinkTarget(IConnector lic);
    void UnLinkTarget();
    void ActiveITarget(bool active);
}

public interface IConnector
{
    int connectorID { get; set; }
    UnityAction<bool> linkSelectAction { get; set; }
    ITarget GetTargetByIndex(int targetID);
    void ConnectToComponent(IConnector connecterPoint, int targetID);

    void AddLinkSelectListener(UnityAction<bool> actionFunction);
    void RemoveLinkSelectListener(UnityAction<bool> uafactionFunction);
}

public interface ICoreComponent
{

}