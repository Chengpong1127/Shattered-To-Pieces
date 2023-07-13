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

    List<ElementDataPack> elements;
    int pageCount;


    private void Awake() {
        elements = new List<ElementDataPack>();
        ElementDataPack dp = new ElementDataPack();
        ElementDescription dsc = new ElementDescription();
        dsc.description = "this is a description for test.";
        dsc.img = spriteList[2];

        dp.img = spriteList[2];
        dp.price = 123;
        dp.description = dsc;
        dp.onClickAction = () => { Debug.Log("Call Click Action."); };

        elements.Add(dp);

        pageCount = 0;

        UpDateDisplayList();
    }

    public void SwitchShopBG(int bgid) {
        if(bgid < 0 || bgid >= spriteList.Count || selfImage == null) { return; }

        selfImage.sprite = spriteList[bgid];
    }

    public void UpDateDisplayList() {
        int elementCount = pageCount * ComponentDisplayList.Count;
        int componentListId = 0;
        while(elementCount < elements.Count && componentListId < ComponentDisplayList.Count) {
            ComponentDisplayList[componentListId].SetData(elements[elementCount]);

            elementCount++;
            componentListId++;
        }
    }
}
