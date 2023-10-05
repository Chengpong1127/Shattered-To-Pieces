using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBar : MonoBehaviour
{
    [SerializeField] List<Label> labels;

    Animator sideBarAnimator;
    bool IsDisplay = false;

    private void Awake() {
        sideBarAnimator = GetComponent<Animator>();

        // setting label variables.
        int labelID = -1;
        labels.ForEach(label => {
            label.sideBar = this;
            label.LabelID = labelID++;
        });
    }



    public void OnClickLabel(int id) {
        if(id == -1) { SwitchSideBarDisplay(); return; }
    }
    public void SwitchSideBarDisplay() {
        if (IsDisplay) { sideBarAnimator.SetTrigger("Display"); }
        else { sideBarAnimator.SetTrigger("Hide"); }
    }
}
