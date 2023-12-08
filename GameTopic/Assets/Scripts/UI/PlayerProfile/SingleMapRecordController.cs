using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class SingleMapRecordController : MonoBehaviour
{
    [SerializeField] private Text MapNameText;
    [SerializeField] private Text RecordText;
    [SerializeField] private Sprite[] MedalSprite;
    [SerializeField] private Image Background;
    [SerializeField] private GameObject particle;
    [SerializeField] private RainbowText rainbowText;
    [SerializeField] private GameObject medal;
    public void SetRecord(string mapName, int record)
    {
        rainbowText.StarRainbow();
        MapNameText.text = mapName;
        RecordText.text = record.ToString();
        switch (record){
            case 0:
                break;
            case 1:
            case 2:
                Background.sprite = MedalSprite[0];
                break;
            case 3:
            case 4:
                Background.sprite = MedalSprite[1];
                break;
            default:
                var p=Instantiate(particle);
                p.transform.SetParent(medal.transform, false);
                Background.sprite = MedalSprite[2];
                break;
        }
    }
}