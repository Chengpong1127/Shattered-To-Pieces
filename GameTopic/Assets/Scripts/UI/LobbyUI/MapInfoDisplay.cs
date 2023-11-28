using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapInfoDisplay : MonoBehaviour
{
    [SerializeField]
    private Image mapImage;
    [SerializeField]
    private TMP_Text mapName;
    [SerializeField]
    private TMP_Text mapDescription;
    void Awake()
    {
        Debug.Assert(mapImage != null);
        Debug.Assert(mapName != null);
        Debug.Assert(mapDescription != null);
    }
    public void SetMapInfo(MapInfo mapInfo){
        
        mapImage.sprite = mapInfo.MapImage;
        mapName.text = mapInfo.MapName;
        mapDescription.text = mapInfo.MapDescription;
    }
}