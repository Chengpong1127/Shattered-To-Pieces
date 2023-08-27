using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class FormalAssemblyRoom : MonoBehaviour, IAssemblyRoom
{

    #region Events
    /// <summary>
    /// Event for when the room mode is changed.
    /// </summary>
    public event Action<AssemblyRoomMode> OnSetRoomMode;
    /// <summary>
    /// Triggered after the device is loaded.
    /// </summary>
    public event Action OnLoadedDevice;
    /// <summary>
    /// Triggered after the device is saved.
    /// </summary> <summary>
    public event Action OnSavedDevice;

    #endregion
    
    #region Properties

    public int PlayerInitMoney { get; set; } = 1000;

    /// <summary>
    /// The device that the FormalAssemblyRoom is controlling.
    /// </summary>
    public Device ControlledDevice { get; private set;}



    /// <summary>
    /// The manager for assembly system.
    /// </summary>
    public AssemblySystemManager AssemblySystemManager { get; private set;}
    /// <summary>
    /// The manager for recording game component units.
    /// </summary>
    /// <value></value>
    public UnitManager GameComponentsUnitManager { get; private set;}


    public AbilityManager AbilityManager => ControlledDevice?.AbilityManager;

    /// <summary>
    /// The factory for creating game components.
    /// </summary>
    private IGameComponentFactory _gameComponentFactory;

    public IAbilityRebinder AbilityRebinder { get; private set; }

    public List<GameComponentData> GameComponentDataList { get; private set; }

    public AbilityRunner AbilityRunner { get; private set; }

    public int CurrentLoadedDeviceID { get; private set; } = 0;
    private PlayerInput playerInput;
    private GameEffectManager _gameEffectManager;
    private InputActionMap AbilityInputActionMap;

    #endregion

    protected void Awake() {
        NetworkManager.Singleton.StartServer();
        _gameComponentFactory = new NetworkGameComponentFactory();
        GameComponentsUnitManager = new UnitManager();
        playerInput = GetComponent<PlayerInput>();
        ControlledDevice = new Device(_gameComponentFactory);
        LoadDevice(CurrentLoadedDeviceID);
        AbilityRunner = AbilityRunner.CreateInstance(gameObject, ControlledDevice.AbilityManager);

        AssemblySystemManager = AssemblySystemManager.CreateInstance(gameObject, GameComponentsUnitManager, playerInput.currentActionMap.FindAction("Drag"), 45f);
        GameComponentDataList = ResourceManager.Instance.LoadAllGameComponentData();

        SetEventHandler();
        _gameEffectManager = new GameEffectManager();
        
    }

    private void SetEventHandler(){
        this.StartListening(EventName.AssemblySystemManagerEvents.OnGameComponentDraggedStart, new Action<IGameComponent>(UpdateSaveHandler));
        this.StartListening(EventName.AssemblySystemManagerEvents.AfterGameComponentConnected, new Action<IGameComponent>(UpdateSaveHandler));
        this.StartListening(EventName.AbilityRunningEvents.OnLocalStartAbility, new Action<int>(AbilityRunner.StartAbility));
        this.StartListening(EventName.AbilityRunningEvents.OnLocalCancelAbility, new Action<int>(AbilityRunner.CancelAbility));
    }

    private void UpdateSaveHandler(object _){
        ControlledDevice.AbilityManager.UpdateDeviceAbilities();
        SaveCurrentDevice();
    }

    /// <summary>
    /// Get the total cost of all game components from the device root.
    /// </summary>
    /// <returns></returns>
    public int GetDeviceTotalCost() {
        if (ControlledDevice == null) return 0;
        var cost = 0;
        ControlledDevice.ForEachGameComponent((component) => {
            var data = GameComponentDataList.Where((data) => data.ResourcePath == component.ComponentName);
            Debug.Assert(data.Count() == 1, "GameComponentDataList should have data for data name: " + component.ComponentName + " but it doesn't.");
            cost += data.First().Price;
        });
        return cost;
    
    }

    private void ClearAllGameComponents()
    {
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var component = unit as IGameComponent;
            component.DisconnectFromParent();
        });
        GameComponentsUnitManager.ForEachUnit((unit) => {
            var component = unit as IGameComponent;
            component.BodyTransform.GetComponent<NetworkObject>()?.Despawn();
        });
        GameComponentsUnitManager.Clear();
    }

    private IDevice LoadNewDevice(DeviceInfo deviceInfo)
    {
        AbilityInputActionMap?.Dispose();

        deviceInfo ??= ResourceManager.Instance.LoadDefaultDeviceInfo();
        ClearAllGameComponents();
        ControlledDevice.Load(deviceInfo);
        ControlledDevice.ForEachGameComponent(GameComponentsUnitManager.AddUnit);

        AbilityInputActionMap = deviceInfo.AbilityManagerInfo.GetAbilityInputActionMap();
        AbilityInputActionMap.Enable();
        AbilityRebinder = new AbilityRebinder(ControlledDevice.AbilityManager, AbilityInputActionMap);
        AbilityRebinder.OnFinishRebinding += _ => SaveCurrentDevice();
        AbilityManager.OnSetAbilityOutOfEntry += _ => SaveCurrentDevice();
        AbilityManager.OnSetAbilityToEntry += _ => SaveCurrentDevice();

        OnLoadedDevice?.Invoke();
        this.TriggerEvent(EventName.AssemblyRoomEvents.OnLoadedDevice);
        return ControlledDevice;
    }

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        var path = componentData.ResourcePath;
        var newComponent = _gameComponentFactory.CreateGameComponentObject(path);
        GameComponentsUnitManager.AddUnit(newComponent);
        newComponent.DraggableTransform.position = position;
        return newComponent;
    }

    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type)
    {
        var filteredList = GameComponentDataList.Where((data) => data.Type == type && data.DisplayAtShop == true).ToList();
        return filteredList;
    }
    
    #region Save and Load Implementation

    public void SaveCurrentDevice(){
        var info = ControlledDevice.Dump();
        var deviceInfo = info as DeviceInfo;
        Debug.Assert(deviceInfo != null);
        ResourceManager.Instance.SaveLocalDeviceInfo(deviceInfo, CurrentLoadedDeviceID.ToString());
        OnSavedDevice?.Invoke();
        this.TriggerEvent(EventName.AssemblyRoomEvents.OnSavedDevice);
    }
    public void LoadDevice(int DeviceID){
        CurrentLoadedDeviceID = DeviceID;
        var deviceInfo = ResourceManager.Instance.LoadLocalDeviceInfo(DeviceID.ToString());
        LoadNewDevice(deviceInfo);
    }
    #endregion

    public int GetPlayerRemainedMoney() {
        return PlayerInitMoney - GetDeviceTotalCost();
    }
    public void SetRoomMode(AssemblyRoomMode mode)
    {
        switch(mode){
            case AssemblyRoomMode.ConnectionMode:
                AssemblySystemManager.EnableAssemblyComponents();
                break;
            case AssemblyRoomMode.PlayMode:
                AssemblySystemManager.DisableAssemblyComponents();
                break;
        }
        OnSetRoomMode?.Invoke(mode);
        this.TriggerEvent(EventName.AssemblyRoomEvents.OnSetRoomMode, mode);
    }
    private void OnDestroy() {
        this.StopListening(EventName.AssemblySystemManagerEvents.OnGameComponentDraggedStart, new Action<IGameComponent>(UpdateSaveHandler));
        this.StopListening(EventName.AssemblySystemManagerEvents.AfterGameComponentConnected, new Action<IGameComponent>(UpdateSaveHandler));
    }
}

public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}
