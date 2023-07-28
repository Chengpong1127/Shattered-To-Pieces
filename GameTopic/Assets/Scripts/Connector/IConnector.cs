using UnityEngine;
using System.Collections.Generic;
public interface IConnector: IDumpable<IInfo>
{
    public IGameComponent GameComponent { get; }
    GameObject GetTargetObjByIndex(int targetID);
    void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info);
    void Disconnect();
    public void SetConnectMode(bool display);
    public void SetSelectingMode(bool selectingMode);
    /// <summary>
    /// Get the first available connector and target ID.
    /// </summary>
    /// <returns> The first available connector and target ID. </returns>
    (IConnector, int) GetAvailableConnector();

    public IList<IConnector> GetChildConnectors();
    public IConnector GetParentConnector();
    public IList<IConnector> ChildConnectors { get; set; }
    public IConnector ParentConnector { get; set; }
}