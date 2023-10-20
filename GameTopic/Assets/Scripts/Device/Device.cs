using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// Device of the game. Must assign GameComponentFactory after Initialize.
/// </summary>
public class Device: IDevice
{
    public event Action OnDeviceConnectionChanged;
    public event Action OnDeviceDied;
    public IGameComponentFactory GameComponentFactory { get; private set; }
    public IGameComponent RootGameComponent { get; private set; }
    public AbilityManager AbilityManager { get; private set; }
    public Device(IGameComponentFactory gameComponentFactory){
        GameComponentFactory = gameComponentFactory;
        AbilityManager = new AbilityManager(this);
        GameEvents.GameComponentEvents.OnEntityDied += entity => {
            if (entity.Equals(RootGameComponent)){
                OnDeviceDied?.Invoke();
            }
        };
        GameEvents.GameComponentEvents.OnGameComponentDisconnected += (component, parent) => {
            AbilityManager
                .Where(ability => ability.OwnerGameComponent.Equals(component))
                .ToList()
                .ForEach(ability => ability.AbilitySpec.CancelAbility());
        };
    }
    public IInfo Dump()
    {
        if (RootGameComponent == null) throw new NullReferenceException("RootGameComponent is null");
        var tree = new Tree(RootGameComponent);
        var deviceInfo = new DeviceInfo();
        var (info, nodeMapping) = tree.Dump<GameComponentInfo>();
        deviceInfo.TreeInfo = info;
        var gameComponentIDMapping = new Dictionary<IGameComponent, int>();
        foreach (var (key, value) in nodeMapping){
            Debug.Assert(key is IGameComponent);
            gameComponentIDMapping.Add(key as IGameComponent, value);
        }
        deviceInfo.AbilityManagerInfo = new AbilityManagerInfo(AbilityManager, gameComponentIDMapping);
        return deviceInfo;
    }
    public async UniTask LoadAsync(IInfo info, Vector3 position){
        if (info is not DeviceInfo){
            throw new ArgumentException("The info should be DeviceInfo");
        }
        if (GameComponentFactory == null){
            throw new NullReferenceException("Device cannot load without GameComponentFactory");
        }
        var deviceInfo = info as DeviceInfo;

        var tempDictionary = CreateAllComponents(deviceInfo.TreeInfo.NodeInfoMap, position);
        tempDictionary.Values.ToList().ForEach((component) => {
            component.SetSelected(true);
        });
        deviceInfo.TreeInfo.NodeInfoMap.ToList().ForEach((pair) => {
            var component = tempDictionary[pair.Key];
            component.Load(pair.Value);
        });
        await UniTask.WaitForFixedUpdate();
        RootGameComponent = tempDictionary[deviceInfo.TreeInfo.rootID];
        await ConnectAllComponents(tempDictionary, deviceInfo.TreeInfo.NodeInfoMap, deviceInfo.TreeInfo.EdgeInfoList); 
        await UniTask.NextFrame();
        tempDictionary.Values.ToList().ForEach((component) => {
            component.SetSelected(false);
        });
        AbilityManager.Load(this, deviceInfo.AbilityManagerInfo, tempDictionary);
        RootGameComponent.OnRootConnectionChanged += () => {
            AbilityManager.UpdateDeviceAbilities();
            OnDeviceConnectionChanged?.Invoke();
        };
        Debug.Assert(RootGameComponent is IDeviceRoot);
        (RootGameComponent as IDeviceRoot).Device = this;
    }

    private Dictionary<int, IGameComponent> CreateAllComponents(Dictionary<int, GameComponentInfo> nodes, Vector3 position){
        Debug.Assert(GameComponentFactory != null, "GameComponentFactory is null");
        var tempDictionary = new Dictionary<int, IGameComponent>();
        foreach (var (key, value) in nodes){
            var componentInfo = value;
            var component = GameComponentFactory.CreateGameComponentObject(componentInfo.ComponentName, position);
            tempDictionary.Add(key, component);
        }
        return tempDictionary;
    }

    private async UniTask ConnectAllComponents(Dictionary<int, IGameComponent> nodes, Dictionary<int, GameComponentInfo> infos, List<(int, int)> edges){
        foreach (var (parent, child) in edges){
            var parentComponent = nodes[parent];
            var childComponent = nodes[child];
            var childInfo = infos[child];
            childComponent.ConnectToParent(parentComponent, childInfo.ConnectionInfo);
            await UniTask.NextFrame();
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
            if(component == null)
                return;
            abilityList.AddRange((component as BaseCoreComponent).GameComponentAbilities);
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
