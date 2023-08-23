using Fusion;
using UnityEngine;


public class NetworkGameComponentFactory : IGameComponentFactory
{

    public NetworkGameComponentFactory(NetworkRunner runner)
    {
        Runner = runner;
    }
    private NetworkRunner Runner;
    public IGameComponent CreateGameComponentObject(string gameComponentName)
    {
        GameObject prefab = ResourceManager.Instance.LoadPrefab(gameComponentName);
        if (prefab != null)
        {
            var obj = Runner.Spawn(prefab);
            if (obj == null)
            {
                Debug.LogWarning("Cannot spawn prefab: " + gameComponentName);
                return null;
            }
            var component = obj.GetComponent<IGameComponent>();
            component.ComponentName = gameComponentName;
            return component;
        }
        else
        {
            Debug.LogWarning("Cannot find prefab: " + gameComponentName);
        }
        return null;
    }
}