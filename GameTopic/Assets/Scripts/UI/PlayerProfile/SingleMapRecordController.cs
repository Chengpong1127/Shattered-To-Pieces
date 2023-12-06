using UnityEngine;
using UnityEngine.UI;

public class SingleMapRecordController : MonoBehaviour
{
    [SerializeField] private Text MapNameText;
    [SerializeField] private Text RecordText;
    [SerializeField] private Sprite[] Medal;
    [SerializeField] private Image Background;
    [SerializeField] private GameObject particle;
    public void SetRecord(string mapName, int record)
    {
        MapNameText.text = mapName;
        RecordText.text = record.ToString();
        switch (record){
            case 0:
                particle.SetActive(false);
                break;
            case 1:
            case 2:
                particle.SetActive(false);
                Background.sprite = Medal[0];
                break;
            case 3:
            case 4:
                particle.SetActive(false);
                Background.sprite = Medal[1];
                break;
            default:
                particle.SetActive(true);
                particle.GetComponent<ParticleSystem>().Play();
                Background.sprite = Medal[2];
                break;
        }
    }
}