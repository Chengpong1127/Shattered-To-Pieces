using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TempSaver : MonoBehaviour
{
    private Device DeviceObject;
    private IGameComponentFactory GameComponentFactory;
    private IInfo SavedInfo;
    private DeviceInfo DefaultInfo(){
        var info = new DeviceInfo();
        info.treeInfo = new TreeInfo();
        info.treeInfo.rootID = 0;
        info.treeInfo.NodeInfoMap = new Dictionary<int, IInfo>();
        info.treeInfo.EdgeInfoList = new List<(int, int)>();
        info.treeInfo.NodeInfoMap.Add(0, new GameComponentInfo{
            componentGUID = 0,
            connectionInfo = ConnectionInfo.NoConnection()
        });
        return info;
    }
    private void Start() {
        GameComponentFactory = gameObject.AddComponent<DeviceFactory>();
        Clear();
        
    }
    public void Create(int id){
        var component = GameComponentFactory.CreateGameComponentObject(id);
    }

    public void Save(){
        Debug.Log("Save");
        SavedInfo = DeviceObject.Dump();
        Debug.Log(toJson(SavedInfo));
    }
    public void Clear(){
        Debug.Log("Clear");
        SavedInfo = DefaultInfo();
        load();
    }
    public void load(){
        if(DeviceObject != null){
            Destroy(DeviceObject.gameObject);
            Destroy(DeviceObject);
        }
            
        CleanAllGameComponent();
        DeviceObject = new GameObject().AddComponent<Device>();
        DeviceObject.GameComponentFactory = GameComponentFactory;
        DeviceObject.Load(SavedInfo);
    }
    private void CleanAllGameComponent(){
        var Devices = GameObject.FindObjectsOfType<Device>();
        foreach(var device in Devices){
            Destroy(device.gameObject);
        }
        var components = GameObject.FindObjectsOfType<GameComponent>();
        foreach(var component in components){
            Destroy(component.gameObject);
        }
        
    }

    private string toJson(IInfo info){
        return JsonConvert.SerializeObject(info);
    }
}
