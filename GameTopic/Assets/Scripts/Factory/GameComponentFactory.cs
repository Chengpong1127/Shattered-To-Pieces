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

    public GameObject CreateComponent(int componentGUID)
    {
        GameObject component;// = new GameObject();
        if (componentGUID == 0)
        {
            component = Instantiate(ComponentPrefab0);
        }
        else if (componentGUID == 1)
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
        GameComponent.ComponentGUID = componentGUID;
        return component;
    }
}
