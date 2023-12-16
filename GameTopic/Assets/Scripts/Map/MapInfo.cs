using UnityEngine;

[CreateAssetMenu(menuName = "MapInfo")]
public class MapInfo : ScriptableObject
{
    public string MapName;
    public string MapSceneName;
    public Sprite MapImage;
    [TextArea(3, 10)]
    public string MapDescription;
    public bool IsRankingMap = false;
    public int MapPlayerCount;
    public bool Available;
    public AudioClip BackgroundMusic;
    [Range(0, 1)]
    public float BackgroundMusicVolume = 1;
}