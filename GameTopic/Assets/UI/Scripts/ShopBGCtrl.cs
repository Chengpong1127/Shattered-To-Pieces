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

    List<GameComponentData>[] componentList;
    int pageCount;
    int currentlementType;


    private void Awake() {
        int typeCount = Enum.GetValues(typeof(GameComponentType)).Length;

        componentList = new List<GameComponentData>[typeCount];
        for(int i = 0; i < typeCount; ++i) {
            componentList[i] = new List<GameComponentData>();
        }

        pageCount = 0;
        currentlementType = 0;

        GameComponentData cd = ScriptableObject.CreateInstance<GameComponentData>(); // new GameComponentData();
        cd.DisplayName = "BaBa";
        cd.DisplayImage = spriteList[0];
        cd.Description = "this is a description for ComponentData test.";
        cd.Price = 5566;
        cd.Type = GameComponentType.Basic;
        cd.ResourcePath = string.Empty;



        componentList[0].Add(cd);

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
    }

    public void SetShopElementClickAction(UnityAction<GameComponentData> ua) {
        ComponentDisplayList.ForEach(e => {
            e.SetClickAction(ua);
        });
    }
}
