using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameComponentFactory : MonoBehaviour
{
    public static GameComponentFactory Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public GameObject ComponentPrefab0;
    public GameObject ComponentPrefab1;

    public GameObject CreateComponent(int componentID)
    {
        var component = new GameObject();
        if (componentID == 0)
        {
            component = Instantiate(ComponentPrefab0);
        }
        else if (componentID == 1)
        {
            component = Instantiate(ComponentPrefab1);
        }
        else{
            Debug.LogError("Component ID not found");
            return null;
        }


        var GameComponent = component.GetComponent<IGameComponent>();
        if (GameComponent == null)
        {
            Debug.LogError("GameComponent not found");
            return null;
        }
        GameComponent.ComponentID = componentID;
        return component;
    }
}
