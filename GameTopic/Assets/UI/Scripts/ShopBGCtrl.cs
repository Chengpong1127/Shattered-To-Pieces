using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBGCtrl : MonoBehaviour
{
    [SerializeField] Image selfImage;
    [SerializeField] List<Sprite> spriteList;
    [SerializeField] List<GameObject> ComponentDisplayList;

    public void SwitchShopBG(int bgid) {
        if(bgid < 0 || bgid > spriteList.Count || selfImage == null) { return; }

        selfImage.sprite = spriteList[bgid];
    }
}
