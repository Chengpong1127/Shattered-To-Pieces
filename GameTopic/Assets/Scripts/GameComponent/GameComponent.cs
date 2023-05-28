using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponent : MonoBehaviour, IGameComponent
{
    private int _gameComponentID;
    private IConnector connector;
    private ICoreComponent coreComponent;

    public IConnector Connector => connector;
    public ICoreComponent CoreComponent => coreComponent;


    public int ComponentID
    {
        get => _gameComponentID;
        set => _gameComponentID = value;
    }

    public void Connect(IGameComponent otherComponent, int targetID)
    {
        connector.ConnectToComponent(otherComponent.Connector, targetID);
    }

    public void DumpInfo(){

    }
}
