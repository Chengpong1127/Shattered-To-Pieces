using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SideBar : MonoBehaviour
{
    [SerializeField] List<Label> labels;
    [SerializeField] Animator LabelStatusAni;
    [SerializeField] List<TitleTextSprite> TitleTextSprites;
    [SerializeField] TitleTextImage TitleTextImage;
    [SerializeField] List<Image> DarkRenderImage;
    [SerializeField] List<Image> LightRenderImage;
    [SerializeField] public List<SellElement> Sells;

    Animator sideBarAnimator;
    int displayTypeID = 0;
    bool IsDisplaying = true;
    bool IsSwitching = false;
    public GameComponentType displayComponentType = GameComponentType.Attack;

    private void Awake() {
        sideBarAnimator = GetComponent<Animator>();

        // setting label variables.
        int ID = 0;
        labels.ForEach(label => {
            label.sideBar = this;
            label.LabelID = ID++;
            label.EnterHover += () => { LabelStatusAni.SetTrigger("EnterHover"); };
            label.ExitHover += () => { LabelStatusAni.SetTrigger("ExitHover"); };
        });

        ID = 0;
        // setting SellElement ID;
        Sells.ForEach(se => {
            se.SellID = ID++;
        });
    }



    public void OnClickLabel(int id) {
        if (id == 4) { sideBarAnimator.SetTrigger("Slide"); IsDisplaying = !IsDisplaying; return; } // 4 is list position of unity editor where display label is.
        if (displayTypeID == id && IsDisplaying) { return; }
        displayTypeID = id;
        if (IsSwitching) { return; }
        IsSwitching = true;
        StartCoroutine(SwitchSideBarEnumerator());
    }
    IEnumerator SwitchSideBarEnumerator() {
        sideBarAnimator.SetTrigger("Switch");
        yield return new WaitWhile(() => sideBarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Switching"));
        yield return new WaitWhile(() => !sideBarAnimator.GetCurrentAnimatorStateInfo(0).IsName("Switching"));

        // Ste SideBar color.
        UpdateSlideBarColor();
        UpdateTitleText();
        UpdateSellElements();

        // finish changing.
        IsSwitching = false;
    }

    void UpdateSlideBarColor() {
        DarkRenderImage.ForEach(img => {
            img.color = labels[displayTypeID].labelColor.Dark;
        });
        LightRenderImage.ForEach(img => {
            img.color = labels[displayTypeID].labelColor.Light;
        });
    }
    void UpdateTitleText() {
        TitleTextImage.FirstCharacter.sprite = TitleTextSprites[displayTypeID].FirstCharacter;
        TitleTextImage.OtherCharacter.sprite = TitleTextSprites[displayTypeID].OtherCharacter;
    }

    public Func<GameComponentType, List<GameComponentData>> GetSells; // should set by someone has Iassemblyroom. It stored componentDatas.
    public void UpdateSellElements() {
        switch (displayTypeID) { // this switch should mach batween GameComponentType and labelID;
            case 0:
                displayComponentType = GameComponentType.Attack; break;
            case 1:
                displayComponentType = GameComponentType.Basic; break;
            case 2:
                displayComponentType = GameComponentType.Functional; break;
            case 3:
                displayComponentType = GameComponentType.Movement; break;
        }

        var sellsData = GetSells?.Invoke(displayComponentType);
        if (sellsData == null) { return; }

        int index = 0;
        while(index < Sells.Count && index < sellsData.Count) {
            Sells[index].SetDisplay(sellsData[index].DisplayImage, sellsData[index].Price);
            index++;
        }
        while(index < Sells.Count) {
            Sells[index].SetDisplay(null, 0);
            index++;
        }
    }
}



[System.Serializable]
public struct TitleTextSprite {
    public Sprite FirstCharacter;
    public Sprite OtherCharacter;
}

[System.Serializable]
public struct TitleTextImage {
    public Image FirstCharacter;
    public Image OtherCharacter;
}