using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillDragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    
    [SerializeField] Transform canvasTransform;
    [SerializeField] TMP_Text displayText;
    Image displayImg;
    RectTransform selfRectTransform;
    LayoutElement selfLayout;
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
        displayImg.raycastTarget = false;
        Dropper = null;
        // transform.SetParent(canvasTransform, false);
        selfLayout.ignoreLayout = true;
    }

    public void OnEndDrag(PointerEventData eventData) {
        displayImg.raycastTarget = true;

        if (Dropper != null) { Dropper.AddSkill(skillData); }
        else { NonSetDropper.AddSkill(skillData); }

        selfLayout.ignoreLayout = false;
        OwnerDropper.RefreshDraggerDisplay();
    }
}
