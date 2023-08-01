using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour, IDevice
{
    public IGameComponentFactory GameComponentFactory;
    public IGameComponent RootGameComponent { set; get; }

    public IInfo Dump()
    {
        Debug.Assert(RootGameComponent != null, "RootGameComponent is null");
        var tree = new Tree(RootGameComponent);
        var deviceInfo = new DeviceInfo();
        deviceInfo.treeInfo = tree.Dump<GameComponentInfo>();
        return deviceInfo;
    }

    public void Load(IInfo info)
    {
        Debug.Assert(info is DeviceInfo);
        var deviceInfo = info as DeviceInfo;

        var tempDictionary = createAllComponents(deviceInfo.treeInfo.NodeInfoMap);
        foreach (var (key, value) in deviceInfo.treeInfo.NodeInfoMap){
            var componentInfo = value as GameComponentInfo;
            var component = tempDictionary[key];
            component.Load(componentInfo);
        }

        foreach (var (key, value) in deviceInfo.treeInfo.NodeInfoMap){
            var component = tempDictionary[key];
            Debug.Log(component.ComponentName);
            component.SetZRotation();
        }

        RootGameComponent = tempDictionary[deviceInfo.treeInfo.rootID];

        connectAllComponents(tempDictionary, deviceInfo.treeInfo.NodeInfoMap, deviceInfo.treeInfo.EdgeInfoList);

        
    }

    private Dictionary<int, IGameComponent> createAllComponents(Dictionary<int, GameComponentInfo> nodes){
        Debug.Assert(GameComponentFactory != null, "GameComponentFactory is null");
        var tempDictionary = new Dictionary<int, IGameComponent>();
        foreach (var (key, value) in nodes){
            var componentInfo = value as GameComponentInfo;
            var component = GameComponentFactory.CreateGameComponentObject(componentInfo.ComponentName);
            tempDictionary.Add(key, component);
        }
        return tempDictionary;
    }

    private void connectAllComponents(Dictionary<int, IGameComponent> nodes, Dictionary<int, GameComponentInfo> infos, List<(int, int)> edges){
        foreach (var (from, to) in edges){
            var fromComponent = nodes[from];
            var toComponent = nodes[to];
            var toInfo = infos[to] as GameComponentInfo;
            
            toComponent.ConnectToParent(fromComponent, toInfo.ConnectionInfo);
        }
    }

    public List<Ability> getAbilityList(){
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
