using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponentFactory : MonoBehaviour
{
    public GameObject ComponentPrefab;

    public GameObject CreateComponent(int componentID)
    {
        var component = Instantiate(ComponentPrefab);
        var GameComponent = component.GetComponent<IGameComponent>();
        
        return component;
    }
}
