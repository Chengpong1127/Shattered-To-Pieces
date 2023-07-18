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
        info.treeInfo = new TreeInfo<GameComponentInfo>();
        info.treeInfo.rootID = 0;
        info.treeInfo.NodeInfoMap = new Dictionary<int, GameComponentInfo>();
        info.treeInfo.EdgeInfoList = new List<(int, int)>();
        info.treeInfo.NodeInfoMap.Add(0, new GameComponentInfo{
            ComponentName = "Square",
            connectionInfo = ConnectionInfo.NoConnection()
        });
        return info;
    }
    private void Start() {
        GameComponentFactory = gameObject.AddComponent<GameComponentFactory>();
        Clear();
        
    }
    public void Create(int id){
        var component = GameComponentFactory.CreateGameComponentObject(id);
    }

    public void Save(){
        Debug.Log("Save");
        SavedInfo = DeviceObject.Dump();
        Debug.Log(DeviceObject.getAbilityList().Count);
    }
    public void Clear(){
        Debug.Log("Clear");

        load(DefaultInfo());
    }
    public void Load(){
        Debug.Log("Load");
        load(SavedInfo);
    }
    public void load(IInfo info){
        if(DeviceObject != null){
            Destroy(DeviceObject.gameObject);
            Destroy(DeviceObject);
        }
            
        CleanAllGameComponent();
        DeviceObject = new GameObject().AddComponent<Device>();
        DeviceObject.GameComponentFactory = GameComponentFactory;
        DeviceObject.Load(info);
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
