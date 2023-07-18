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
