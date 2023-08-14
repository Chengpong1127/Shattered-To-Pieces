using UnityEngine;
using System.Collections.Generic;
public interface IConnector: IDumpable<IInfo>
{
    public IGameComponent GameComponent { get; }
    Target GetTarget(int targetID);
    public void SetNonConnectedTargetsDisplay(bool display);
    public void SetAllTargetDisplay(bool display);

    /// <summary>
    /// Get the first available connector and target ID.
    /// </summary>
    /// <returns> The first available connector and target ID. </returns>
    (IConnector, int) GetAvailableConnector();
    void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info);
    void Disconnect();
}