using System.Collections.Generic;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    Dictionary<ConnecterPoint, ConnecterPoint> ConnecterMap { get; }
}

public interface IConnector
{
    
}

public interface ICoreComponent
{

}