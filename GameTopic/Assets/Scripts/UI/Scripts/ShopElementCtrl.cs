using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ShopElementCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button selfButton;
    [SerializeField] Image componentImg;
    [SerializeField] PriceCtrl priceCtrl;


    [Header("Pointer Event function object")]
    [SerializeField] DescriptionBoxCtrl boxCtrl;

    GameComponentData componentData;

    UnityAction<GameComponentData> onClickAction;

    Vector2 maxSpriteSize { get; set; } = new Vector2(0,0);
    RectTransform componentImgRectTransform;

    private void Awake() {
        maxSpriteSize = componentImg.rectTransform.sizeDelta;
        componentImgRectTransform = componentImg.rectTransform;
    }

    /// <summary>
    /// Set game component data to shop element.
    /// </summary>
    /// <param name="cd"></param>
    public void SetData(GameComponentData cd) {
        componentData = cd;
        UpDateElement();
    }

    public void InvokeOnClickAction() {
        onClickAction(componentData);
    }

    public void SetClickAction(UnityAction<GameComponentData> oCA) {
        selfButton.onClick.RemoveListener(InvokeOnClickAction);
        onClickAction = oCA;
        selfButton.onClick.AddListener(InvokeOnClickAction);
    }

    /// <summary>
    /// Update UI to show the latest component data.
    /// </summary>
    public void UpDateElement() {
        Vector2 newSpriteSize = componentData.DisplayImage.rect.size / componentImg.pixelsPerUnit;
        float sizeScale = maxSpriteSize.x / newSpriteSize.x;
        if(maxSpriteSize.y < newSpriteSize.y * sizeScale) {
            sizeScale = maxSpriteSize.y / newSpriteSize.y;
        }

        componentImgRectTransform.sizeDelta = newSpriteSize * sizeScale;
        componentImg.sprite = componentData.DisplayImage;
        priceCtrl.SetPrice(componentData.Price);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(boxCtrl == null) { return; } 
        boxCtrl.SetDescriptionData(componentData.Description, componentData.DisplayImage);
        boxCtrl.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (boxCtrl == null) { return; }
        boxCtrl.gameObject.SetActive(false);
    }
}
