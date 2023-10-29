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
    private GameComponent Owner = null;

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
        GameEvents.UIEvents.OnGameComponentAbilitySelected.Invoke(Owner);
    }

    public void OnEndDrag(PointerEventData eventData) {
        GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd.Invoke(Owner);
        displayImg.raycastTarget = true;
        DASO = null; // clear skilldata avoid duplicate skill appear.

        if (Dropper != null) { Dropper.AddSkill(OwnerDropper.BoxID, draggerID); }
        else { NonSetDropper?.AddSkill(OwnerDropper.BoxID, draggerID); }

        selfLayout.ignoreLayout = false;
    }

    public void UpdateDisplay(DisplayableAbilityScriptableObject newData) {
        DASO = newData;
        if (DASO == null) { ShowDisplay(false); return; }
        displayImg.sprite = DASO.Image;
        ShowDisplay(true);
    }
    public void SetOwner(GameComponent owner){
        Owner = owner;
    }
    void ShowDisplay(bool b) {
        gameObject.SetActive(b);
        // if (b) {
        //     displayImg.color = Color.white;
        //     displayImg.raycastTarget = true;
        // } else {
        //     displayImg.color = Color.clear;
        //     displayImg.raycastTarget = false;
        // }
    }
}
