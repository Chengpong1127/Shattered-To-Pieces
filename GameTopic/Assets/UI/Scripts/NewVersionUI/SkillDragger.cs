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
    public GameComponentAbility skillData { get; set; } = null;


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

        var data = skillData;
        skillData = null; // clear skilldata avoid duplicate skill appear.

        if (Dropper != null) { Dropper.AddSkill(OwnerDropper.BoxID, data); }
        else { NonSetDropper?.AddSkill(OwnerDropper.BoxID, data); }

        selfLayout.ignoreLayout = false;
    }

    public void UpdateDisplay(GameComponentAbility newData) {
        skillData = newData;
        if (skillData == null) { gameObject.SetActive(false); return; }
        var DASO = skillData.AbilityScriptableObject as DisplayableAbilityScriptableObject;

        displayImg.sprite = DASO != null ? DASO.Image : null;
        gameObject.SetActive(true);
    }
}
