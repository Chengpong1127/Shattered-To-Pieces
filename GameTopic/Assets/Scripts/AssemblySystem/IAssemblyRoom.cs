using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAssemblyRoom
{
    /// <summary>
    /// Create a new game component in the assembly room
    /// </summary>
    /// <param name="componentID"></param>
    /// <param name="position"></param>
    public void CreateNewGameComponent(GameComponentData componentData, Vector2 position);

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
    public List<GameComponentData> GetGameComponentDataList(GameComponentType type);

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