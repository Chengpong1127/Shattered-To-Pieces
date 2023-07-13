using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public struct ElementDescription {
    // public string name { get; set; }
    public string description { get; set; }
    public Sprite img { get; set; }
}
public class DescriptionBoxCtrl : MonoBehaviour
{
    [SerializeField] Image selfImage;
    [SerializeField] Image componentImage;
    [SerializeField] TMP_Text componentTMP;
    [SerializeField] List<Sprite> spriteList;

    ElementDescription descriptionData;

    public void SetDescriptionData(ElementDescription ed) {
        descriptionData = ed;
        UpDateDescription();
    }

    public void UpDateDescription() {
        if(componentImage == null || componentTMP == null) { return; }

        componentImage.sprite = descriptionData.img;
        componentTMP.text = descriptionData.description;
    }

    public void SwitchBoxBG(int bgid) {
        if (bgid < 0 || bgid >= spriteList.Count || componentImage == null) { return; }

        componentImage.sprite = spriteList[bgid];
    }
}
