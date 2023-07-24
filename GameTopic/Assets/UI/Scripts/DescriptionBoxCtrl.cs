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

    Vector2 maxSpriteSize { get; set; } = new Vector2(0, 0);
    RectTransform componentImgRectTransform;

    private void Awake() {
        maxSpriteSize = componentImage.rectTransform.sizeDelta;
        componentImgRectTransform = componentImage.rectTransform;
    }

    public void SetDescriptionData(string nText, Sprite nImg) {
        descriptionText = nText;
        img = nImg;
        UpDateDescription();
    }

    /// <summary>
    /// Update description UI to show the latest component data.
    /// </summary>
    public void UpDateDescription() {
        if(componentImage == null || componentTMP == null) { return; }

        Vector2 newSpriteSize = img.rect.size / img.pixelsPerUnit;
        float sizeScale = maxSpriteSize.x / newSpriteSize.x;
        if (maxSpriteSize.y < newSpriteSize.y * sizeScale) {
            sizeScale = maxSpriteSize.y / newSpriteSize.y;
        }

        componentImgRectTransform.sizeDelta = newSpriteSize * sizeScale;
        componentImage.sprite = img;
        componentTMP.text = descriptionText;
    }

    /// <summary>
    /// Change description UI's background for different type of element.
    /// </summary>
    /// <param name="bgid"></param>
    public void SwitchBoxBG(int bgid) {
        if (bgid < 0 || bgid >= spriteList.Count || selfImage == null) { return; }

        selfImage.sprite = spriteList[bgid];
    }
}
