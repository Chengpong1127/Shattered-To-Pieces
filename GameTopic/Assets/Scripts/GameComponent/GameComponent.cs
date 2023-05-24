using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponent : MonoBehaviour, IGameComponent
{
    public int GameComponentID;
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

    public void DumpInfo(){

    }
}
