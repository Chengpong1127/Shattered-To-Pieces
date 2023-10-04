using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAssemblyRoom
{
    public Device ControlledDevice { get; }


    /// <summary>
    /// Create a new game component in the assembly room
    /// </summary>
    /// <param name="componentID"></param>
    /// <param name="position"></param>
    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position);
    /// <summary>
    /// Get all game component data of the specified type.
    /// </summary>
    /// <param name="type">Type specified.</param>
    /// <returns></returns>
    public List<GameComponentData> GetGameComponentDataListByTypeForShop(GameComponentType type);
    

    /// <summary>
    /// The AssemblySystemManager of the assembly room.
    /// </summary>
    /// <value></value>
    public AssemblyController assemblyController { get; }

    #region Money

    /// <summary>
    /// The init money of the player.
    /// </summary>
    /// <value></value>
    public int PlayerInitMoney { get; set; }

    /// <summary>
    /// The remaining money of the player from the init money.
    /// </summary>
    /// <value></value>
    public int GetPlayerRemainedMoney();
    /// <summary>
    /// Get the total cost of the device.
    /// </summary>
    /// <value></value>
    public int GetDeviceTotalCost();

    #endregion

    #region Save and Load
    /// <summary>
    /// Save the current device to the device list. Save by the loaded id.
    /// </summary>
    public void SaveCurrentDevice();

    /// <summary>
    /// Load the device from the device list.
    /// </summary>
    /// <param name="DeviceID"></param>
    public void LoadDevice(int DeviceID);

    /// <summary>
    /// The event that will be triggered after loading a device.
    /// </summary>
    public event Action OnLoadedDevice;
    /// <summary>
    /// The event that will be triggered after saving a device.
    /// </summary>
    public event Action OnSavedDevice;


    #endregion

    #region Ability

    /// <summary>
    /// Get the AbilityManager of the device.
    /// </summary>
    /// <value></value>
    public AbilityManager AbilityManager { get; }

    /// <summary>
    /// The AbilityKeyChanger of the device.
    /// </summary>
    /// <value> The AbilityKeyChanger of the device.</value>
    public IAbilityRebinder AbilityRebinder { get; }
    /// <summary>
    /// Start to listen to player's input to change the ability button.
    /// </summary>
    /// <param name="abilityButtonID"></param>

    #endregion
}

/// <summary>
/// All the types of the game component.
/// </summary>
public enum GameComponentType{
    Basic,
    Attack,
    Movement,
    Functional
}   