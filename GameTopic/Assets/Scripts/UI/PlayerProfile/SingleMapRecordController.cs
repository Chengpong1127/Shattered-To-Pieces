using UnityEngine;
using UnityEngine.UI;

public class SingleMapRecordController : MonoBehaviour
{
    [SerializeField] private Text MapNameText;
    [SerializeField] private Text RecordText;

    public void SetRecord(string mapName, int record)
    {
        MapNameText.text = mapName;
        RecordText.text = record.ToString();
    }
}