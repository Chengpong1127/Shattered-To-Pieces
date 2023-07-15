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
    public void CreateNewGameComponent(int componentID, Vector2 position);

    /// <summary>
    /// Set the assembly room to the specified mode. There are two modes: AssemblyMode and PlayMode.
    /// </summary>
    /// <param name="mode">The mode specified.</param>
    public void SetRoomMode(AssemblyRoomMode mode);
    public void GetGameComponentList(GameComponentType type);

    public void SaveCurrentDevice(string DeviceName);
    public void LoadDevice(string DeviceName);
    public List<string> GetSavedDeviceList();
}

public enum GameComponentType{
    Basic,

}