using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopBGCtrl : MonoBehaviour
{
    [SerializeField] Image selfImage;
    [SerializeField] List<Sprite> spriteList;
    [SerializeField] List<ShopElementCtrl> ComponentDisplayList;

    List<ElementDataPack>[] elements;
    int pageCount;
    int currentlementType;


    private void Awake() {
        elements = new List<ElementDataPack>[4];
        for(int i = 0; i < 4; ++i) {
            elements[i] = new List<ElementDataPack>();
        }

        ElementDataPack dp = new ElementDataPack();
        ElementDescription dsc = new ElementDescription();
        dsc.description = "this is a description for test.";
        dsc.img = spriteList[2];

        dp.img = spriteList[2];
        dp.price = 123;
        dp.description = dsc;
        dp.onClickAction = () => { Debug.Log("Call Click Action."); };

        pageCount = 0;

        currentlementType = 0;
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        currentlementType = 1;
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        currentlementType = 2;
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        currentlementType = 3;
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);
        elements[currentlementType].Add(dp);

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
        while(elementCount < elements[currentlementType].Count && componentListId < ComponentDisplayList.Count) {
            ComponentDisplayList[componentListId].SetData(elements[currentlementType][elementCount]);
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
        if((pageCount + 1) * ComponentDisplayList.Count >= elements[currentlementType].Count) { return; }
        pageCount++;
        UpDateDisplayList();
    }
    public void PrevPage() {
        if(pageCount == 0) { return; }
        pageCount--;
        UpDateDisplayList();
    }
}
