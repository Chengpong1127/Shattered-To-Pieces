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
    public AssemblyController assemblyController { get; private set;}
    /// <summary>
    /// The manager for recording game component units.
    /// </summary>
    /// <value></value>
    public List<IGameComponent> SpawnedGameComponents = new();


    public AbilityManager AbilityManager => ControlledDevice?.AbilityManager;

    /// <summary>
    /// The factory for creating game components.
    /// </summary>
    private IGameComponentFactory _gameComponentFactory;

    public IAbilityRebinder AbilityRebinder { get; private set; }

    public List<GameComponentData> GameComponentDataList {
        get {
            _gameComponentDataList ??= ResourceManager.Instance.LoadAllGameComponentData();
            return _gameComponentDataList;
        }
    }
    private List<GameComponentData> _gameComponentDataList;
    private AbilityRunner AbilityRunner;

    public int CurrentLoadedDeviceID { get; private set; } = 0;
    private InputActionMap AbilityInputActionMap;
    private GameEffectManager gameEffectManager;
    private PlayerInput playerInput;

    #endregion

    protected void Awake() {
        NetworkManager.Singleton.StartServer();
        playerInput = GetComponent<PlayerInput>();
        _gameComponentFactory = new NetworkGameComponentFactory();
        ControlledDevice = new Device(_gameComponentFactory);
        LoadDevice(CurrentLoadedDeviceID);
        AbilityRunner = AbilityRunner.CreateInstance(gameObject, ControlledDevice.AbilityManager, 0);
        assemblyController = GetComponent<AssemblyController>();
        // assemblyController.ServerInitialize(
        //     () => SpawnedGameComponents.Select((component) => component.NetworkObjectID).ToArray(),
        //     () => SpawnedGameComponents.Select((component) => component.NetworkObjectID).ToArray(),
        //     playerInput.currentActionMap.FindAction("DragComponent"),
        //     playerInput.currentActionMap.FindAction("FlipComponent"),
        //     playerInput.currentActionMap.FindAction("RotateComponent")
        // );

        SetEventHandler();
        gameEffectManager = new GameEffectManager();
        
    }

    private void SetEventHandler(){
        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += AbilityRunner.StartEntryAbility;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += AbilityRunner.CancelEntryAbility;
        assemblyController.OnGameComponentSelected += _ => UpdateAbility();
        assemblyController.OnGameComponentSelectedEnd += _ => UpdateAbility();

    }

    private void UpdateAbility(){
        ControlledDevice.AbilityManager.UpdateDeviceAbilities();
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
        SpawnedGameComponents.ForEach((component) => {
            component.DisconnectFromParent();
        });
        SpawnedGameComponents.ForEach((component) => {
            (component as Entity)?.Die();
        });

        SpawnedGameComponents.Clear();
    }

    private IDevice LoadNewDevice(DeviceInfo deviceInfo)
    {
        AbilityInputActionMap?.Dispose();

        deviceInfo ??= ResourceManager.Instance.LoadDefaultDeviceInfo();
        ClearAllGameComponents();
        //ControlledDevice.Load(deviceInfo);
        ControlledDevice.ForEachGameComponent(SpawnedGameComponents.Add);

        AbilityInputActionMap = deviceInfo.AbilityManagerInfo.GetAbilityInputActionMap();
        AbilityInputActionMap.Enable();
        //AbilityRebinder = new AbilityRebinder(ControlledDevice.AbilityManager, AbilityInputActionMap);

        OnLoadedDevice?.Invoke();
        GameEvents.AssemblyRoomEvents.OnLoadedDevice.Invoke();
        return ControlledDevice;
    }

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        var path = componentData.ResourcePath;
        var newComponent = _gameComponentFactory.CreateGameComponentObject(path, Vector3.zero);
        SpawnedGameComponents.Add(newComponent);
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
        GameEvents.AssemblyRoomEvents.OnSavedDevice.Invoke();
    }
    public void LoadDevice(int DeviceID){
        if (ControlledDevice?.RootGameComponent != null) SaveCurrentDevice();
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
                assemblyController.enabled = true;
                break;
            case AssemblyRoomMode.PlayMode:
                assemblyController.enabled = false;
                break;
        }
        OnSetRoomMode?.Invoke(mode);
        GameEvents.AssemblyRoomEvents.OnSetRoomMode.Invoke(mode);
    }
    void OnApplicationQuit()
    {
        SaveCurrentDevice();
    }
}

public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}
