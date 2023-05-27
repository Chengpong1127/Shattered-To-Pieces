using System.Collections.Generic;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    Dictionary<ConnecterPoint, ConnecterPoint> ConnectorMap { get; }
}

public interface IConnector
{
    void ConnectToComponent(IConnector connecterPoint, int targetID);
}

public interface ICoreComponent
{

}