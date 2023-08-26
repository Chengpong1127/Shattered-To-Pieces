using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameComponentFactory : IGameComponentFactory
{
    public GameComponentFactory()
    {
    }
    public IGameComponent CreateGameComponentObject(string gameComponentName)
    {
        GameObject prefab = ResourceManager.Instance.LoadPrefab(gameComponentName);
        if (prefab != null)
        {
            var obj = GameObject.Instantiate(prefab);
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
