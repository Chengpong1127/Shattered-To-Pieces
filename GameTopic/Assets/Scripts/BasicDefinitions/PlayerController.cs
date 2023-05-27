using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
public interface IPlayerController{
    void CreateDeviceInstance();
    void SaveComponentCount(Dictionary<String, int> counts);

    Dictionary<String, int> LoadComponentCount();

}
public class SaveData
{
    public string playerId;
    public Dictionary<String, int> ComponentCounts;

    public SaveData(string playerId, Dictionary<String, int> ComponentCounts)
    {
        this.playerId = playerId;
        this.ComponentCounts = ComponentCounts;
    }
}