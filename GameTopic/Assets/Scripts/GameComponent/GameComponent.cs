using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponent : MonoBehaviour, IGameComponent
{
    private int _gameComponentID;
    private IConnector connector;
    private ICoreComponent coreComponent;

    public IConnector Connector
    {
        get
        {
            return connector;
        }
    }

    public ICoreComponent CoreComponent
    {
        get
        {
            return coreComponent;
        }
    }

    public Dictionary<ConnecterPoint, ConnecterPoint> ConnectorMap => throw new System.NotImplementedException();

    public int ComponentID => _gameComponentID;

    public void DumpInfo(){

    }
}
