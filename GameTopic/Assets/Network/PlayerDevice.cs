using System.Data.SqlTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Newtonsoft.Json;

public class PlayerDevice : NetworkBehaviour
{
    private Device device;
    private IGameComponentFactory factory;
    void Awake()
    {
        
        
    }
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void LoadDevice_RPC(string infoData){
        Debug.Log("LoadDevice_RPC");
        device = new Device(factory);
        var info = JsonConvert.DeserializeObject<DeviceInfo>(infoData);
        device.Load(info);
    }
    public override void Spawned()
    {
        factory = new NetworkGameComponentFactory(Runner);
        if (HasInputAuthority){
            var info = ResourceManager.Instance.LoadLocalDeviceInfo("0") ?? ResourceManager.Instance.LoadDefaultDeviceInfo();
            string infoData = JsonConvert.SerializeObject(info);
            LoadDevice_RPC(infoData);
        }
        
    }
}
