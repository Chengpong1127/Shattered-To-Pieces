using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Label : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [field:SerializeField] public LabelColor labelColor;
    public SideBar sideBar { get; set; }
    public int LabelID { get; set; }

    Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Click label : " + gameObject.name);

        // call sideBar click label function.
        sideBar?.OnClickLabel(LabelID);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        animator?.SetTrigger("PointerEnter");
    }
    public void OnPointerExit(PointerEventData eventData) {
        animator?.SetTrigger("PointerExit");
    }
}


[System.Serializable]
public struct LabelColor {
    [field: SerializeField] public Color Dark { get;set; }
    [field: SerializeField] public Color Light { get; set; }
}
