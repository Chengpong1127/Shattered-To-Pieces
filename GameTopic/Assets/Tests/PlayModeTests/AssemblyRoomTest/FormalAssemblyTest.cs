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
        roomManager.SaveCurrentDevice();
        roomManager.LoadDevice(roomManager.CurrentLoadedDeviceID);
        Assert.True(roomManager.ControlledDevice != null);
        var deviceInfo2 = roomManager.ControlledDevice.Dump() as DeviceInfo;
        Assert.True(CompareDeviceInfo(deviceInfo, deviceInfo2));
    }

    [Test]
    public void CreateNewGameComponentTest(){
        var roomManager = new GameObject().AddComponent<FormalAssemblyRoom>();
        var deviceInfo = roomManager.ControlledDevice.Dump() as DeviceInfo;
        GameComponentData data = ScriptableObject.CreateInstance<GameComponentData>();
        data.DisplayName = "Square";
        data.ResourcePath = "Square";

        roomManager.CreateNewGameComponent(data, new Vector2(0, 0));
        Assert.AreEqual(roomManager.SpawnedGameComponents.Count, deviceInfo.TreeInfo.NodeInfoMap.Count + 1);
        
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
        var testMoney = 100;
        var currentMoney = roomManager.GetPlayerRemainedMoney();

        var device = roomManager.ControlledDevice;

        var test_data = ScriptableObject.CreateInstance<GameComponentData>();
        test_data.ResourcePath = "Square";
        test_data.Price = testMoney;
        roomManager.GameComponentDataList.Add(test_data);
        var connection_info = new ConnectionInfo{
            linkedTargetID = 0
        };
        var new_component = roomManager.CreateNewGameComponent(test_data, new Vector2(0, 0));
        new_component.ConnectToParent(device.RootGameComponent, connection_info);
        Assert.True(roomManager.GetPlayerRemainedMoney() == currentMoney - testMoney);

    }
    

    private bool CompareDeviceInfo(DeviceInfo info1, DeviceInfo info2){
        if(info1.TreeInfo.rootID != info2.TreeInfo.rootID) return false;
        if(info1.TreeInfo.NodeInfoMap.Count != info2.TreeInfo.NodeInfoMap.Count) return false;
        if(info1.TreeInfo.EdgeInfoList.Count != info2.TreeInfo.EdgeInfoList.Count) return false;
        foreach(var node in info1.TreeInfo.NodeInfoMap){
            if(!info2.TreeInfo.NodeInfoMap.ContainsKey(node.Key)) return false;
        }
        foreach(var edge in info1.TreeInfo.EdgeInfoList){
            if(!info2.TreeInfo.EdgeInfoList.Contains(edge)) return false;
        }

        return true;

    }
}
