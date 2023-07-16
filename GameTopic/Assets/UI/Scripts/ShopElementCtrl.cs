using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct ElementDataPack {
    public Sprite img { get; set; }
    public int price { get; set; }
    public ElementDescription description { get; set; }
    public UnityAction onClickAction { get; set; }
}


public class ShopElementCtrl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button selfButton;
    [SerializeField] Image componentImg;
    [SerializeField] PriceCtrl priceCtrl;


    [Header("Pointer Event function object")]
    [SerializeField] DescriptionBoxCtrl boxCtrl;

    ElementDataPack dataPack;
    UnityAction onClickAction;

    private void Awake() {
        dataPack.onClickAction = () => { };
    }

    public void SetData(ElementDataPack dp) {
        // selfButton.onClick.RemoveListener(dataPack.onClickAction);
        dataPack = dp;
        // selfButton.onClick.AddListener(dataPack.onClickAction);
        UpDateElement();
    }

    public void SetClickAction(UnityAction oCA) {
        selfButton.onClick.RemoveListener(onClickAction);
        onClickAction = oCA;
        selfButton.onClick.AddListener(onClickAction);
    }

    public void UpDateElement() {
        componentImg.sprite = dataPack.img;
        priceCtrl.SetPrice(dataPack.price);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if(boxCtrl == null) { return; } 
        boxCtrl.SetDescriptionData(dataPack.description);
        boxCtrl.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (boxCtrl == null) { return; }
        boxCtrl.gameObject.SetActive(false);
    }
}
