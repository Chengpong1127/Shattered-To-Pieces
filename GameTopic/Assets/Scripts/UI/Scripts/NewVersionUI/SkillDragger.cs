using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SkillDragger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
    
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Image displayImg;
    [SerializeField]
    private TMP_Text EnergyCostText;
    [SerializeField]
    private RectTransform selfRectTransform;
    [SerializeField]
    private LayoutElement selfLayout;
    [HideInInspector]
    public Transform DraggingParentTransform;
    public SkillDropper Dropper { get; set; } = null;
    public SkillDropper OwnerDropper { get; set; } = null;
    public SkillDropper NonSetDropper { get; set; } = null;
    private DisplayableAbilityScriptableObject DASO { get;set; } = null;
    public int draggerID { get; set; } = -1;
    private GameComponent Owner = null;

    private void Awake() {
        Debug.Assert(canvasGroup != null);
        Debug.Assert(selfRectTransform != null);
        Debug.Assert(displayImg != null);
        Debug.Assert(selfLayout != null);
        Debug.Assert(EnergyCostText != null);
    }

    public void Show() {
        displayImg.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.2f);
    }
    public void Hide() {
        displayImg.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0.5f, 0.2f);
        
    }

    public void OnDrag(PointerEventData eventData) {
        selfRectTransform.anchoredPosition += eventData.delta;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        Dropper = null;
        canvasGroup.blocksRaycasts = false;
        selfLayout.ignoreLayout = true;
        GameEvents.UIEvents.OnGameComponentAbilitySelected.Invoke(Owner);
    }

    public void OnEndDrag(PointerEventData eventData) {
        GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd.Invoke(Owner);
        canvasGroup.blocksRaycasts = true;
        DASO = null; // clear skilldata avoid duplicate skill appear.

        if (Dropper != null) { Dropper.AddSkill(OwnerDropper.SelfBoxID, draggerID); }
        else { NonSetDropper?.AddSkill(OwnerDropper.SelfBoxID, draggerID); }
        selfLayout.ignoreLayout = false;
        
    }

    public void SetSkill(DisplayableAbilityScriptableObject newData) {
        DASO = newData;
        if (DASO == null) {
            gameObject.SetActive(false);
        }else{
            if (displayImg.sprite != DASO.Image){
                displayImg.sprite = DASO.Image;
                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            }
            EnergyCostText.text = DASO.EnergyCost.ToString();
            gameObject.SetActive(true);
        }
        
    }
    public void SetOwner(GameComponent owner){
        Owner = owner;
    }

}
