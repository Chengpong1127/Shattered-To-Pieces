using UnityEngine;

[CreateAssetMenu(menuName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    public string MapName;
    public string MapSceneName;
    public Sprite MapImage;
    [TextArea(3, 10)]
    public string MapDescription;
    public int MapPlayerCount;
    public bool Available;
}