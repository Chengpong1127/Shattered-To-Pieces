using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class DeviceFactory : MonoBehaviour
{
    private PlayerController playerController;
    private Dictionary<String, int> prefabCounts;
    public enum GameObjectType
    {
        Square,
        Tri
    }
    public static DeviceFactory Instance { get; private set; }
    private void Awake()
    {
        prefabCounts = new Dictionary<String, int>();
        playerController =new PlayerController();
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public DeviceInfo exampleInfo(){
        DeviceInfo info = new DeviceInfo();
        info.GameComponentInfoMap.Add(0, new GameComponentInfo{componentGUID = 0, connectorInfo = new ConnectorInfo{linkedConnectorID = -1, linkedTargetID = -1}});
        info.GameComponentInfoMap.Add(1, new GameComponentInfo{componentGUID = 1, connectorInfo = new ConnectorInfo{linkedConnectorID = 0, linkedTargetID = 0}});
        info.GameComponentInfoMap.Add(2, new GameComponentInfo{componentGUID = 1, connectorInfo = new ConnectorInfo{linkedConnectorID = 0, linkedTargetID = 1}});
        return info;
    }

    public void SaveDevice()
    {
        playerController.SaveComponentCount(prefabCounts);
    }
    public void CreateDevice()
    {
        var Devices = GameObject.FindObjectsOfType<Device>();
        foreach (var device in Devices)
        {
            Destroy(device.gameObject);
        }
        var components = GameObject.FindObjectsOfType<GameComponent>();
        foreach (var component in components)
        {
            Destroy(component.gameObject);
        }
        prefabCounts = playerController.LoadComponentCount();
        Dictionary<string, int> prefabCountsCopy = new Dictionary<string, int>(prefabCounts);
        //Debug.Log("prefabCounts: " + (prefabCounts == null ? "null" : prefabCounts.Count.ToString()));
        foreach (var kvp in prefabCountsCopy)
        {
            String prefabName = kvp.Key;
            int count = kvp.Value;
            Debug.Log(kvp.Key + kvp.Value);
            GameObject prefab = Resources.Load<GameObject>(prefabName);
            for (int i = 0; i < count; i++)
            {
                Instantiate(prefab);
                prefab.transform.position = new Vector3(0, 5, 0);
            }
        }
    }
    public GameObject CreateGameObject(int GameObjectID)
    {
        foreach (var i in Enum.GetValues(typeof(GameObjectType)))
        {
            if ((int)i == GameObjectID)
            {
                GameObject prefab = Resources.Load<GameObject>(i.ToString());
                if (prefab != null)
                {
                    var obj = Instantiate(prefab);
                    if (prefabCounts.ContainsKey(i.ToString()))
                    {
                        prefabCounts[i.ToString()]++;
                    }
                    else
                    {
                        prefabCounts.Add(i.ToString(), 1);
                    }
                    return obj;
                }
                else
                {
                    Debug.Log("Prefab not found: " + i.ToString());
                }
                prefab.transform.position = new Vector3(0, 5, 0);
            }
        }
        return null;
    }
}
