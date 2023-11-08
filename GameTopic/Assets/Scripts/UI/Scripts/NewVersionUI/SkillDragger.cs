using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

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
        transform.DOScale(Vector3.one * 1.2f, 0.2f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd.Invoke(Owner);
        displayImg.raycastTarget = true;
        DASO = null; // clear skilldata avoid duplicate skill appear.

        if (Dropper != null) { Dropper.AddSkill(OwnerDropper.BoxID, draggerID); }
        else { NonSetDropper?.AddSkill(OwnerDropper.BoxID, draggerID); }

        selfLayout.ignoreLayout = false;
        transform.DOScale(Vector3.one, 0.2f);
    }

    public void UpdateDisplay(DisplayableAbilityScriptableObject newData) {
        DASO = newData;
        if (DASO == null) { gameObject.SetActive(false); return; }
        if (displayImg.sprite != DASO.Image){
            displayImg.sprite = DASO.Image;
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
        }
        gameObject.SetActive(true);
    }
    public void SetOwner(GameComponent owner){
        Owner = owner;
    }
}
