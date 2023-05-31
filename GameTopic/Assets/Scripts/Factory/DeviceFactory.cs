using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceFactory : MonoBehaviour
{
    public static DeviceFactory Instance { get; private set; }
    private void Awake()
    {
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
    public GameObject CreateDevice(DeviceInfo info)
    {
        var device = new GameObject();
        var deviceComponent = device.AddComponent<Device>();
        deviceComponent.LoadDevice(info);
        return device;
    }
}
