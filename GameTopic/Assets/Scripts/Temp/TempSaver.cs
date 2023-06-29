using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSaver : MonoBehaviour
{
    private Device DeviceObject;
    private DeviceInfo DefaultInfo(){
        var info = new DeviceInfo();
        info.GameComponentInfoMap.Add(0, new GameComponentInfo{
            componentGUID = 0,
            connectorInfo = new ConnectionInfo{
                linkedTargetID = -1,
            }
        });
        return info;
    }
    private void Start() {
        for(int i = 0; i < 3; i++){
            infos[i] = DefaultInfo();
        }
        load(0);
    }
    public void Create0(){
        var component = GameComponentFactory.Instance.CreateComponent(0);
        component.transform.position = new Vector3(0, 5, 0);
    }
    public void Create1(){
        var component = GameComponentFactory.Instance.CreateComponent(1);
        component.transform.position = new Vector3(0, 5, 0);
    }
    private int currentDeviceID = 0;
    private DeviceInfo[] infos = new DeviceInfo[3];
    public void ChooseInfo(int id){
        currentDeviceID = id;
        load(id);
    }
    public void Save(){
        Debug.Log("Save");
        infos[currentDeviceID] = DeviceObject.DumpDevice();
        infos[currentDeviceID].printAllInfo();
    }
    public void Clear(){
        Debug.Log("Clear");
        infos[currentDeviceID] = DefaultInfo();
        load(currentDeviceID);
    }
    public void load(int id){
        if(DeviceObject != null){
            Destroy(DeviceObject.gameObject);
            Destroy(DeviceObject);
        }
            
        CleanAllGameComponent();
        var info = infos[id];
        //var gameobj = DeviceFactory.Instance.CreateDevice(info);
       // DeviceObject = gameobj.GetComponent<Device>();
       // gameobj.transform.position = new Vector3(0, 5, 0);
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
}
