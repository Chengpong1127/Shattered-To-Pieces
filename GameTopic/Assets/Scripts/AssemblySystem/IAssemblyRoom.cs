using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IAssemblyRoom
{
    /// <summary>
    /// Event that will be triggered after switching the assembly room mode.
    /// </summary>
    public event Action<AssemblyRoomMode> OnSetRoomMode;


    /// <summary>
    /// Create a new game component in the assembly room
    /// </summary>
    /// <param name="componentID"></param>
    /// <param name="position"></param>
    public IGameComponent CreateNewGameComponent(GameComponentData componentData, Vector2 position);

    /// <summary>
    /// Set the assembly room to the specified mode. There are two modes: AssemblyMode and PlayMode.
    /// </summary>
    /// <param name="mode">The mode specified.</param>
    public void SetRoomMode(AssemblyRoomMode mode);
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
    public AssemblySystemManager AssemblySystemManager { get; }

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
    /// Save the current device to the device list.
    /// </summary>
    /// <param name="DeviceName">The name of the saved device.</param>
    public void SaveCurrentDevice(string DeviceName);

    /// <summary>
    /// Load the device from the device list.
    /// </summary>
    /// <param name="DeviceName">The name of the device to be loaded.</param>
    /// <returns> The loaded device.</returns>
    public void LoadDevice(string DeviceName);

    /// <summary>
    /// Load the device from the device list and set the position of the device.
    /// </summary>
    /// <param name="DeviceName"> The name of the device to be loaded.</param>
    /// <param name="position"> The spawning position of the device.</param>
    /// <returns> The loaded device.</returns>
    public void LoadDevice(string DeviceName, Vector2 position);

    /// <summary>
    /// Rename the device in the device list.
    /// </summary>
    /// <param name="DeviceName"> The name of the device to be renamed.</param>
    /// <param name="NewDeviceName"> The new name of the device.</param>
    public void RenameDevice(string DeviceName, string NewDeviceName);

    /// <summary>
    /// Get the list of the saved and available devices.
    /// </summary>
    /// <returns> The list of the saved and available devices.</returns>
    public List<string> GetSavedDeviceList();


    #endregion

    #region Ability

    /// <summary>
    /// Get the AbilityManager of the device.
    /// </summary>
    /// <value></value>
    public AbilityManager AbilityManager { get; }
    
    /// <summary>
    /// The AbilityRunner of the device.
    /// </summary>
    /// <value></value>
    public AbilityRunner AbilityRunner { get; }

    /// <summary>
    /// Start to listen to player's input to change the ability button.
    /// </summary>
    /// <param name="abilityButtonID"></param>
    public void StartChangeAbilityKey(int abilityButtonID);
    /// <summary>
    /// Stop listening to player's input to change the ability button.
    /// </summary>
    public void EndChangeAbilityKey();
    /// <summary>
    /// Event that will be triggered when the ability button is changed.
    /// </summary>
    public event Action<string> OnFinishChangeAbilityKey;

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