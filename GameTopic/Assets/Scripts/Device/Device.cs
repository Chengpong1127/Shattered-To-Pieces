using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        Debug.Assert(RootGameComponent != null, "RootGameComponent is null");
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

        foreach (var (key, _) in deviceInfo.treeInfo.NodeInfoMap){
            var component = tempDictionary[key];
            component.SetZRotation();
        }

        RootGameComponent = tempDictionary[deviceInfo.treeInfo.rootID];

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
        foreach (var (from, to) in edges){
            var fromComponent = nodes[from];
            var toComponent = nodes[to];
            var toInfo = infos[to];
            
            toComponent.ConnectToParent(fromComponent, toInfo.ConnectionInfo);
        }
    }

    public List<Ability> GetAbilityList(){
        var abilityList = new List<Ability>();
        if (RootGameComponent == null)
            return abilityList;

        var tree = new Tree(RootGameComponent);
        tree.TraverseBFS((node) => {
            var component = node as IGameComponent;
            Debug.Assert(component != null);
            if(component.CoreComponent == null)
                return;
            var abilities = component.CoreComponent.AllAbilities;
            foreach (var ability in abilities){
                abilityList.Add(ability.Value);
            }
        });
        return abilityList;
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
