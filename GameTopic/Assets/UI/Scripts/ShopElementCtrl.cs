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

    public void UpDateElement() {
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
