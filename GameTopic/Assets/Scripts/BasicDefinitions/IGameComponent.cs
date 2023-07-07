using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;




// Description: Defines the basic interfaces for the game components.
public interface IGameComponent: ITreeNode, IUnit, IDragable
{
    public bool IsInDevice { get; }
    public int ComponentGUID { get; set; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    public void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info);
    public void DisconnectFromParent();
    public (IGameComponent, ConnectionInfo) GetAvailableConnection();
    public void SetAssemblyMode(bool assemblyMode);
    public void SetAvailableForConnection(bool draggingMode);
}

public interface IConnector: IDumpable<IInfo>
{
    public IGameComponent GameComponent { get; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info);
    void Disconnect();
    public void SetConnectMode(bool display);
    public void SetSelectingMode(bool selectingMode);
    (IConnector, int) GetAvailableConnector(); // return the first available connector and target ID

    public IList<IConnector> GetChildConnectors();
    public IConnector GetParentConnector();
    public IList<IConnector> ChildConnectors { get; set; }
    public IConnector ParentConnector { get; set; }

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
