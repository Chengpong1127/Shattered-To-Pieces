using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Newtonsoft.Json;

public class PlayerDevice : NetworkBehaviour
{
    public Device SelfDevice { get; private set; }
    public IGameComponentFactory GameComponentFactory { get; private set; }
    [ServerRpc]
    private void LoadDeviceServerRpc(string json)
    {
        SelfDevice = new Device(GameComponentFactory);
        SelfDevice.Load(DeviceInfo.CreateFromJson(json));
    }
    void Awake()
    {
        GameComponentFactory = new NetworkGameComponentFactory();
    }
    void Start()
    {
        
        if (IsOwner){
            DeviceInfo info = GetLocalDeviceInfo();
            LoadDeviceServerRpc(info.ToJson());
        }
        
    }

    private DeviceInfo GetLocalDeviceInfo(){
        return ResourceManager.Instance.LoadLocalDeviceInfo("0") ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
    }
}
