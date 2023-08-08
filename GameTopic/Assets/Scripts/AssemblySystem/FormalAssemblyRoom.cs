using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameframe.SaveLoad;
using System.Linq;
using System;
using UnityEngine.InputSystem;

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


    public AbilityManager AbilityManager => ControlledDevice.AbilityManager;

    /// <summary>
    /// The factory for creating game components.
    /// </summary>
    private IGameComponentFactory _gameComponentFactory;

    public IAbilityRebinder AbilityRebinder { get; private set; }

    public List<GameComponentData> GameComponentDataList { get; private set; }

    public AbilityRunner AbilityRunner { get; private set; }

    public int CurrentLoadedDeviceID { get; private set; } = 0;

    private InputManager _inputManager;

    #endregion

    protected void Awake() {
        _gameComponentFactory = GameComponentFactory.CreateInstance(gameObject);
        GameComponentsUnitManager = new UnitManager();
        _inputManager = new InputManager();
        ControlledDevice = new Device(_gameComponentFactory);
        LoadDevice(CurrentLoadedDeviceID);

        AssemblySystemManager = AssemblySystemManager.CreateInstance(gameObject, GameComponentsUnitManager, _inputManager.AssemblyRoom.Drag, 45f);
        AssemblySystemManager.OnGameComponentDraggedStart += _ => UpdateSave();
        AssemblySystemManager.AfterGameComponentConnected += _ => UpdateSave();

        GameComponentDataList = ResourceManager.Instance.LoadAllGameComponentData();

        AbilityRunner = AbilityRunner.CreateInstance(gameObject, ControlledDevice.AbilityManager);
        AbilityRunner.BindInputActionsToRunner(GetAbilityInputActions());

        _inputManager.Enable();
    }
    protected void Start() {
        
        SetRoomMode(AssemblyRoomMode.PlayMode);
    }

    private void UpdateSave(){
        ControlledDevice.AbilityManager.UpdateDeviceAbilities();
        SaveCurrentDevice();
    }
    private InputAction[] GetAbilityInputActions(){
        var abilityInputActions = new InputAction[ControlledDevice.AbilityManager.AbilityInputEntryNumber];
        foreach(var action in _inputManager){
            if (action.name.StartsWith("Ability")){
                var abilityID = int.Parse(action.name[7..]);
                abilityInputActions[abilityID] = action;
            }
        }
        return abilityInputActions;
    }

    /// <summary>
    /// Get the total cost of all game components from the device root.
    /// </summary>
    /// <returns></returns>
    public int GetDeviceTotalCost() {
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
            Destroy(component.BodyTransform.gameObject);
        });
        GameComponentsUnitManager.Clear();
    }

    private IDevice LoadNewDevice(DeviceInfo deviceInfo)
    {


        deviceInfo ??= ResourceManager.Instance.LoadDefaultDeviceInfo();
        ClearAllGameComponents();
        ControlledDevice.Load(deviceInfo);
        ControlledDevice.ForEachGameComponent(GameComponentsUnitManager.AddUnit);


        
        AbilityRebinder = new AbilityRebinder(ControlledDevice.AbilityManager, GetAbilityInputActions());
        AbilityRebinder.OnFinishRebinding += _ => SaveCurrentDevice();

        OnLoadedDevice?.Invoke();
        return ControlledDevice;
    }

    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position)
    {
        var path = componentData.ResourcePath;
        var newComponent = _gameComponentFactory.CreateGameComponentObject(path);
        GameComponentsUnitManager.AddUnit(newComponent);
        newComponent.DragableTransform.position = position;
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
    }
}

public enum AssemblyRoomMode{
    ConnectionMode,
    PlayMode
}
