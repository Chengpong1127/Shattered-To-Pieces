using UnityEngine;

[CreateAssetMenu(menuName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    public string MapName;
    public string MapSceneName;
    public string MapDescription;
    public int MapPlayerCount;
    public bool Available;
}