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
    private void Start() {
        DeviceInfo info = new DeviceInfo();
        info.GameComponentInfoMap.Add(0, new GameComponentInfo{componentGUID = 0, connectorInfo = new ConnectorInfo()});
        info.GameComponentInfoMap.Add(1, new GameComponentInfo{componentGUID = 1, connectorInfo = new ConnectorInfo{linkedConnectorID = 0, linkedTargetID = 2}});
        info.GameComponentInfoMap.Add(2, new GameComponentInfo{componentGUID = 1, connectorInfo = new ConnectorInfo{linkedConnectorID = 0, linkedTargetID = 1}});
        var device = CreateDevice(info);
        device.transform.position = new Vector3(0, 0, 0);

        var d = device.GetComponent<Device>();
        var info2 = d.DumpDevice();
        var device2 = CreateDevice(info2);
        device2.transform.position = new Vector3(0, 0, 10);
    }
    public GameObject CreateDevice(DeviceInfo info)
    {
        var device = new GameObject();
        var deviceComponent = device.AddComponent<Device>();
        deviceComponent.LoadDevice(info);
        return device;
    }
}
