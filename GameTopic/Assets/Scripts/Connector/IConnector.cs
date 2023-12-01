using UnityEngine;
using System.Collections.Generic;
public interface IConnector: IDumpable<IInfo>
{
    public event System.Action OnJointBreak;
    public GameComponent GameComponent { get; }
    Target GetTarget(int targetID);
    public void SetNonConnectedTargetsDisplay(bool display);
    public void SetAllTargetDisplay(bool display);
    void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info);
    void Disconnect();
}