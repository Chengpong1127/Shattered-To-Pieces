using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameComponentFactory : MonoBehaviour, IGameComponentFactory
{
    public IGameComponent CreateGameComponentObject(string gameComponentName)
    {
        GameObject prefab = ResourceManager.Instance.LoadPrefab(gameComponentName);
        if (prefab != null)
        {
            var obj = Instantiate(prefab);
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
