using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionBoxCtrl : MonoBehaviour
{
    [SerializeField] Image selfImage;
    [SerializeField] Image componentImage;
    [SerializeField] TMP_Text componentTMP;
    [SerializeField] List<Sprite> spriteList;

    string descriptionText;
    Sprite img;
    public void SetDescriptionData(string nText, Sprite nImg) {
        descriptionText = nText;
        img = nImg;
        UpDateDescription();
    }

    public void UpDateDescription() {
        if(componentImage == null || componentTMP == null) { return; }

        componentImage.sprite = img;
        componentTMP.text = descriptionText;
    }

    public void SwitchBoxBG(int bgid) {
        if (bgid < 0 || bgid >= spriteList.Count || selfImage == null) { return; }

        selfImage.sprite = spriteList[bgid];
    }
}
