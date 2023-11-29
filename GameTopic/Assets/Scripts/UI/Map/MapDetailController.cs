using UnityEngine;
using UnityEngine.UI;
public class MapDetailController : MonoBehaviour
{
    [SerializeField]
    private Text MapNameText;
    [SerializeField]
    private Text MapDescriptionText;
    [SerializeField]
    private Image MapImage;
    void Awake()
    {
        Debug.Assert(MapNameText != null);
        Debug.Assert(MapDescriptionText != null);
        Debug.Assert(MapImage != null);
    
    }

    public void SetMapInfo(MapInfo mapInfo){
        MapNameText.text = mapInfo.MapName;
        MapDescriptionText.text = mapInfo.MapDescription;
        MapImage.sprite = mapInfo.MapImage;
    }
}