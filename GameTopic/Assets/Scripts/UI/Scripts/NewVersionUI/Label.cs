using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Label : MonoBehaviour, IPointerClickHandler {
    [SerializeField] public LabelColor labelColor;
    public SideBar sideBar { get; set; }
    public int LabelID { get; set; }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Click label : " + gameObject.name);

        // call sideBar click label function.
        sideBar?.OnClickLabel(LabelID);
    }
}


[System.Serializable]
public struct LabelColor {
    public Color Dark { get;set; }
    public Color Light { get; set; }
}
