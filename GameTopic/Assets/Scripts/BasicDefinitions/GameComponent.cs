using System.Collections.Generic;

// Description: Defines the basic interfaces for the game components.
public interface IGameComponent
{
    int ComponentID { get; }
    public IConnector Connector { get; }
    public ICoreComponent CoreComponent { get; }
    void Connect(IGameComponent otherComponent, int targetID);
}

public interface IConnector
{
    void ConnectToComponent(IConnector connecterPoint, int targetID);
}

public interface ICoreComponent
{

}