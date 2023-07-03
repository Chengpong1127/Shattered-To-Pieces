using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GameComponentFactory : MonoBehaviour, IGameComponentFactory
{
    private PlayerController playerController;
    private Dictionary<String, int> prefabCounts;
    public enum GameObjectType
    {
        Square,
        Tri,
        Wheel
    }
    public IGameComponent CreateGameComponentObject(int GameObjectID)
    {
        var objectName = ((GameObjectType)GameObjectID).ToString();
        return CreateGameComponentObject(objectName);
    }
    public IGameComponent CreateGameComponentObject(string gameComponentName)
    {
        GameObject prefab = Resources.Load<GameObject>(gameComponentName);
        if (prefab != null)
        {
            var obj = Instantiate(prefab);
            var component = obj.GetComponent<IGameComponent>();
            return component;
        }
        else
        {
            Debug.LogWarning("Cannot find prefab: " + gameComponentName);
        }
        return null;
    }

}
