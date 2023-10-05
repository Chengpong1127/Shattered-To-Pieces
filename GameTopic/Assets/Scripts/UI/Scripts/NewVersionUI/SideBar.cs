using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBar : MonoBehaviour
{
    [SerializeField] List<Label> labels;

    private void Awake() {

        // setting label variables.
        int labelID = 0;
        labels.ForEach(label => {
            label.sideBar = this;
            label.LabelID = labelID++;
        });
    }



    public void OnClickLabel(int id) {

    }
}
