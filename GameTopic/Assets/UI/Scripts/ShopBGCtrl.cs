using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopBGCtrl : MonoBehaviour
{
    [SerializeField] Image selfImage;
    [SerializeField] List<Sprite> spriteList;
    [SerializeField] List<ShopElementCtrl> ComponentDisplayList;

    List<GameComponentData>[] componentList { get; set; } = new List<GameComponentData>[Enum.GetValues(typeof(GameComponentType)).Length];
    int pageCount;
    int currentlementType;


    private void Awake() {

        pageCount = 0;
        currentlementType = 0;

        UpDateDisplayList();
    }

    public void SwitchShopBG(int bgid) {
        if(bgid < 0 || bgid >= spriteList.Count || selfImage == null || currentlementType == bgid) { return; }

        selfImage.sprite = spriteList[bgid];
        currentlementType = bgid;
        pageCount = 0;

        UpDateDisplayList();
    }

    public void UpDateDisplayList() {
        if(componentList[currentlementType] == null) { return; }

        int elementCount = pageCount * ComponentDisplayList.Count;
        int componentListId = 0;
        while(elementCount < componentList[currentlementType].Count && componentListId < ComponentDisplayList.Count) {  
            ComponentDisplayList[componentListId].SetData(componentList[currentlementType][elementCount]);
            ComponentDisplayList[componentListId].gameObject.SetActive(true);

            elementCount++;
            componentListId++;
        }
        while(componentListId < ComponentDisplayList.Count) {
            ComponentDisplayList[componentListId].gameObject.SetActive(false);
            componentListId++;
        }
    }

    public void NextPage() {
        if((pageCount + 1) * ComponentDisplayList.Count >= componentList[currentlementType].Count) { return; }
        pageCount++;
        UpDateDisplayList();
    }
    public void PrevPage() {
        if(pageCount == 0) { return; }
        pageCount--;
        UpDateDisplayList();
    }


    public void SetElements(List<GameComponentData> cdList, GameComponentType type) {
        componentList[(int)type] = cdList;
        UpDateDisplayList();
    }

    public void SetShopElementClickAction(UnityAction<GameComponentData> ua) {
        ComponentDisplayList.ForEach(e => {
            e.SetClickAction(ua);
        });
    }
}
