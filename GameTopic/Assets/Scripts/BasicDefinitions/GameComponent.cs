using System.Collections.Generic;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    int ComponentID { get; }
    Dictionary<ConnecterPoint, ConnecterPoint> ConnectorMap { get; }
}

public interface IConnector
{
    
}

public interface ICoreComponent
{

}