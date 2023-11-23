using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
public class NetworkGameComponentFactory : IGameComponentFactory
{
    public ulong OwnerClientId { get; private set; }
    public NetworkGameComponentFactory(ulong ownerClientId = 0){
        OwnerClientId = ownerClientId;
    }

    public IGameComponent CreateGameComponentObject(string gameComponentName, Vector3 position)
    {
        GameObject prefab = ResourceManager.Instance.LoadPrefab(gameComponentName);
        if (prefab != null)
        {
            var obj = GameObject.Instantiate(prefab, position, Quaternion.identity);
            obj.GetComponent<NetworkObject>()?.SpawnWithOwnership(OwnerClientId);
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
