using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideBar : MonoBehaviour
{
    [SerializeField] List<Label> labels;
    [SerializeField] List<Image> DarkRenderImage;
    [SerializeField] List<Image> LightRenderImage;

    Animator sideBarAnimator;
    int displayTypeID = 1;
    bool IsSwitching = false;

    private void Awake() {
        sideBarAnimator = GetComponent<Animator>();

        // setting label variables.
        int labelID = 0;
        labels.ForEach(label => {
            label.sideBar = this;
            label.LabelID = labelID++;
        });
    }



    public void OnClickLabel(int id) {
        if (id == 0) { sideBarAnimator.SetTrigger("Slide"); return; }
        if (displayTypeID == id) { return; }
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
}
