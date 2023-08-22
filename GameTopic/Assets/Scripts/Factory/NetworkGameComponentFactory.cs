using Fusion;
using UnityEngine;


public class NetworkGameComponentFactory : IGameComponentFactory
{

    public NetworkGameComponentFactory(NetworkRunner runner, PlayerRef player)
    {
        Runner = runner;
        Player = player;
    }
    private NetworkRunner Runner;
    private PlayerRef Player;
    public IGameComponent CreateGameComponentObject(string gameComponentName)
    {
        GameObject prefab = ResourceManager.Instance.LoadPrefab(gameComponentName);
        if (prefab != null)
        {
            var obj = Runner.Spawn(prefab, inputAuthority: Player);
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