using UnityEngine;
using System.Collections.Generic;
public interface IConnector: IDumpable<IInfo>
{
    public IGameComponent GameComponent { get; }
    Target GetTarget(int targetID);
    public void SetAllTargetsDisplay(bool active);

    /// <summary>
    /// Get the first available connector and target ID.
    /// </summary>
    /// <returns> The first available connector and target ID. </returns>
    (IConnector, int) GetAvailableConnector();
    void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info);
    void Disconnect();
    public IList<IConnector> ChildConnectors { get; }
    public IConnector ParentConnector { get; }
}