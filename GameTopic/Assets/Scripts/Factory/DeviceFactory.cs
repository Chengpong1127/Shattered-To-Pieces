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
        info.GameComponentIDMap = new Dictionary<int, int>();
        info.GameComponentIDMap.Add(0, 0);
        info.GameComponentIDMap.Add(1, 1);
        info.GameComponentIDMap.Add(2, 1);
        info.ConnecterMap = new Dictionary<int, ConnectorInfo>();
        info.ConnecterMap.Add(1, new ConnectorInfo{linkedConnectorID = 0, linkedTargetID = 0});
        info.ConnecterMap.Add(2, new ConnectorInfo{linkedConnectorID = 0, linkedTargetID = 1});
        var device = CreateDevice(info);
        device.transform.position = new Vector3(0, 0, 0);
    }
    public GameObject CreateDevice(IDeviceInfo info)
    {
        var device = new GameObject();
        var deviceComponent = device.AddComponent<Device>();
        deviceComponent.LoadDevice(info);
        return device;
    }
}
