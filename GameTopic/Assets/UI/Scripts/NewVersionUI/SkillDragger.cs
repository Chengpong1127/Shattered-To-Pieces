using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    
    // [SerializeField] TMP_Text displayText;
    Image displayImg;
    RectTransform selfRectTransform;
    LayoutElement selfLayout;
    public Transform DraggingParentTransform;
    public SkillDropper Dropper { get; set; } = null;
    public SkillDropper OwnerDropper { get; set; } = null;
    public SkillDropper NonSetDropper { get; set; } = null;
    public DisplayableAbilityScriptableObject DASO { get;set; } = null;
    public int draggerID { get; set; } = -1;

    private void Awake() {
        selfRectTransform = GetComponent<RectTransform>();
        displayImg = GetComponent<Image>();
        selfLayout = GetComponent<LayoutElement>();
    }

    public void OnDrag(PointerEventData eventData) {
        Vector3 globalMouseePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(selfRectTransform, eventData.position, eventData.pressEventCamera, out globalMouseePos)) {
            selfRectTransform.position = globalMouseePos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Dropper = null;
        displayImg.raycastTarget = false;
        selfLayout.ignoreLayout = true;
    }

    public void OnEndDrag(PointerEventData eventData) {
        displayImg.raycastTarget = true;

        var data = DASO;
        DASO = null; // clear skilldata avoid duplicate skill appear.

        if (Dropper != null) { Dropper.AddSkill(OwnerDropper.BoxID, draggerID); }
        else { NonSetDropper?.AddSkill(OwnerDropper.BoxID, draggerID); }

        selfLayout.ignoreLayout = false;
    }

    public void UpdateDisplay(DisplayableAbilityScriptableObject newData) {
        DASO = newData;
        if (DASO == null) { gameObject.SetActive(false); return; }

        displayImg.sprite = DASO.Image;
        gameObject.SetActive(true);
    }
}
