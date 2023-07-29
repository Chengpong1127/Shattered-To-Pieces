using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FormalAssemblyTest
{
    [Test]
    public void ConstructRoomManagerTest()
    {
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        Assert.True(roomManager != null);
        Assert.True(roomManager.ControlledDevice != null);
        Assert.True(roomManager.ControlledDevice.RootGameComponent != null);
    }

    [Test]
    public void SaveLoadTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        var deviceInfo = roomManager.ControlledDevice.Dump() as DeviceInfo;
        roomManager.SaveCurrentDevice("test");
        roomManager.LoadDevice("test");
        Assert.True(roomManager.ControlledDevice != null);
        var deviceInfo2 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo2));

        roomManager.LoadDevice("test");
        Assert.True(roomManager.ControlledDevice != null);
        deviceInfo2 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo2));

        roomManager.LoadDevice("test");
        Assert.True(roomManager.ControlledDevice != null);
        deviceInfo2 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo2));

        roomManager.LoadDevice("test");
        Assert.True(roomManager.ControlledDevice != null);
        deviceInfo2 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo2));
    }
    [Test]
    public void RenameDeviceTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        var deviceInfo = roomManager.ControlledDevice.Dump() as DeviceInfo;
        roomManager.SaveCurrentDevice("test");
        roomManager.LoadDevice("test");
        Assert.True(roomManager.ControlledDevice != null);
        var deviceInfo2 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(deviceInfo.treeInfo.rootID == deviceInfo2.treeInfo.rootID);
        roomManager.RenameDevice("test", "test2");
        roomManager.LoadDevice("test2");
        var deviceInfo3 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo3));

        roomManager.RenameDevice("test2", "test3");
        roomManager.LoadDevice("test3");
        deviceInfo3 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo3));

    }

    [Test]
    public void GetSavedDeviceListTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        var deviceInfo = roomManager.ControlledDevice.Dump() as DeviceInfo;
        roomManager.SaveCurrentDevice("test");
        roomManager.SaveCurrentDevice("test2");
        roomManager.SaveCurrentDevice("test3");
        var deviceList = roomManager.GetSavedDeviceList();
        foreach(var device in deviceList){
            Debug.Log(device);
        }
        Assert.True(deviceList.Contains("test.json"));
        Assert.True(deviceList.Contains("test2.json"));
        Assert.True(deviceList.Contains("test3.json"));
    }

    [Test]
    public void CreateNewGameComponentTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        var deviceInfo = roomManager.ControlledDevice.Dump() as DeviceInfo;
        GameComponentData data = ScriptableObject.CreateInstance<GameComponentData>();
        data.ResourcePath = "Square";

        roomManager.CreateNewGameComponent(data, new Vector2(0, 0));
        Assert.AreEqual(roomManager.GameComponentsUnitManager.UnitMap.Count, 2);
        
    }

    [Test]
    public void SetRoomModeTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        roomManager.SetRoomMode(AssemblyRoomMode.PlayMode);
        roomManager.SetRoomMode(AssemblyRoomMode.ConnectionMode);
    }

    [Test]
    public void GetGameComponentDataListTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        var dataList = roomManager.GetGameComponentDataListByTypeForShop(GameComponentType.Basic);
        Assert.True(dataList.Count > 0);
        Debug.Log(dataList[0].DisplayName);
    }

    [Test]
    public void PlayerRemainedMoneyTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        Assert.True(roomManager.GetPlayerRemainedMoney() == 1000);

        var device = roomManager.ControlledDevice;

        var test_data = ScriptableObject.CreateInstance<GameComponentData>();
        test_data.DisplayName = "Square";
        test_data.ResourcePath = "Square";
        test_data.Price = 100;
        roomManager.GameComponentDataList.Add(test_data);
        var connection_info = new ConnectionInfo{
            linkedTargetID = 0
        };
        var new_component = roomManager.CreateNewGameComponent(test_data, new Vector2(0, 0));
        new_component.ConnectToParent(device.RootGameComponent, connection_info);

        Assert.True(roomManager.GetDeviceTotalCost() == 100);
        Assert.True(roomManager.GetPlayerRemainedMoney() == 900);

    }
    

    private bool CompareDeviceInfo(DeviceInfo info1, DeviceInfo info2){
        if(info1.treeInfo.rootID != info2.treeInfo.rootID) return false;
        if(info1.treeInfo.NodeInfoMap.Count != info2.treeInfo.NodeInfoMap.Count) return false;
        if(info1.treeInfo.EdgeInfoList.Count != info2.treeInfo.EdgeInfoList.Count) return false;
        foreach(var node in info1.treeInfo.NodeInfoMap){
            if(!info2.treeInfo.NodeInfoMap.ContainsKey(node.Key)) return false;
        }
        foreach(var edge in info1.treeInfo.EdgeInfoList){
            if(!info2.treeInfo.EdgeInfoList.Contains(edge)) return false;
        }
        return true;
    }
}
