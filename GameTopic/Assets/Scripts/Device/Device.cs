using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbilitySystem.Authoring;

/// <summary>
/// Device of the game. Must assign GameComponentFactory after Initialize.
/// </summary>
public class Device: IDevice
{
    public IGameComponentFactory GameComponentFactory { get; private set; }
    public IGameComponent RootGameComponent { get; private set; }
    public AbilityManager AbilityManager { get; private set; }
    public Device(IGameComponentFactory gameComponentFactory){
        GameComponentFactory = gameComponentFactory;
        AbilityManager = new AbilityManager(this);
    }
    public IInfo Dump()
    {
        if (RootGameComponent == null) throw new NullReferenceException("RootGameComponent is null");
        var tree = new Tree(RootGameComponent);
        var deviceInfo = new DeviceInfo();
        var (info, nodeMapping) = tree.Dump<GameComponentInfo>();
        deviceInfo.treeInfo = info;
        var gameComponentIDMapping = new Dictionary<IGameComponent, int>();
        foreach (var (key, value) in nodeMapping){
            Debug.Assert(key is IGameComponent);
            gameComponentIDMapping.Add(key as IGameComponent, value);
        }
        deviceInfo.abilityManagerInfo = new AbilityManagerInfo(AbilityManager, gameComponentIDMapping);
        return deviceInfo;
    }

    public void Load(IInfo info)
    {
        if (info is not DeviceInfo){
            throw new ArgumentException("The info should be DeviceInfo");
        }
        if (GameComponentFactory == null){
            throw new NullReferenceException("Device cannot load without GameComponentFactory");
        }
        var deviceInfo = info as DeviceInfo;

        var tempDictionary = CreateAllComponents(deviceInfo.treeInfo.NodeInfoMap);
        foreach (var (key, value) in deviceInfo.treeInfo.NodeInfoMap){
            var componentInfo = value;
            var component = tempDictionary[key];
            component.Load(componentInfo);
        }
        RootGameComponent = tempDictionary[deviceInfo.treeInfo.rootID];
        foreach (var (key, componentInfo) in deviceInfo.treeInfo.NodeInfoMap){
            var component = tempDictionary[key];
            component.SetZRotation(componentInfo.ConnectionZRotation);
        }
        ConnectAllComponents(tempDictionary, deviceInfo.treeInfo.NodeInfoMap, deviceInfo.treeInfo.EdgeInfoList);
        
        AbilityManager.Load(this, deviceInfo.abilityManagerInfo, tempDictionary);
    }

    private Dictionary<int, IGameComponent> CreateAllComponents(Dictionary<int, GameComponentInfo> nodes){
        Debug.Assert(GameComponentFactory != null, "GameComponentFactory is null");
        var tempDictionary = new Dictionary<int, IGameComponent>();
        foreach (var (key, value) in nodes){
            var componentInfo = value as GameComponentInfo;
            var component = GameComponentFactory.CreateGameComponentObject(componentInfo.ComponentName);
            tempDictionary.Add(key, component);
        }
        return tempDictionary;
    }

    private void ConnectAllComponents(Dictionary<int, IGameComponent> nodes, Dictionary<int, GameComponentInfo> infos, List<(int, int)> edges){
        foreach (var (parent, child) in edges){
            var parentComponent = nodes[parent];
            var childComponent = nodes[child];
            var childInfo = infos[child];
            childComponent.ConnectToParent(parentComponent, childInfo.ConnectionInfo);
        }
    }
    /// <summary>
    /// Get all ability data in the device from root game component.
    /// </summary>
    /// <returns> (abilityList, abilitySpecsList) The information and the instances of abilities. </returns>
    public GameComponentAbility[] GetAbilityData(){
        var abilityList = new List<GameComponentAbility>();
        if (RootGameComponent == null)
            return abilityList.ToArray();

        var tree = new Tree(RootGameComponent);
        tree.TraverseBFS((node) => {
            var component = node as IGameComponent;
            Debug.Assert(component != null);
            if(component.CoreComponent == null)
                return;
            abilityList.AddRange(component.CoreComponent.GameComponentAbilities);
        });
        return abilityList.ToArray();
    }

    public void ForEachGameComponent(Action<IGameComponent> action){
        var tree = new Tree(RootGameComponent);
        tree.TraverseBFS((node) => {
            var component = node as IGameComponent;
            Debug.Assert(component != null);
            action(component);
        });
    }

}
