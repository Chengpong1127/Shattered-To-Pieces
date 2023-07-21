using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCtrl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Transform canvasTransform;
    
    static SkillBoxCtrl NonSetBox;
    public SkillBoxCtrl ownerBox { get; set; }

    RectTransform selfRectTransform;
    Image selfImage;
    SkillBoxCtrl dropObjTarget;

    public Ability skillData { get; set; }

    private void Awake() {
        selfRectTransform  = GetComponent<RectTransform>();
        selfImage = GetComponent<Image>();
        dropObjTarget = null;

        if (selfImage == null) {
            Debug.Log("image not found.");
            gameObject.SetActive(false);
        }

        // NonSetBox = GameObject.Find("NonSetBox").GetComponent<SkillBoxCtrl>();
    }

    private void Start() {
        NonSetBox = GameObject.Find("NonSetBox").GetComponent<SkillBoxCtrl>();
        skillData = Ability.EmptyAbility();
        skillData.AbilityName = selfImage.color.ToString();
    }


    public void UpDateSkillDisplay() {
        // ability should have sprite
    }

    public void SetDropObjectTarget(SkillBoxCtrl obj) {
        dropObjTarget = obj;
    }


    public void OnDrag(PointerEventData eventData) {
        Vector3 globalMouseePos;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(selfRectTransform, eventData.position,eventData.pressEventCamera, out globalMouseePos)) {
            selfRectTransform.position = globalMouseePos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        selfImage.raycastTarget = false;

        dropObjTarget = null;
        transform.SetParent(canvasTransform, false);
    }

    public void OnEndDrag(PointerEventData eventData) {
        selfImage.raycastTarget = true;
        ownerBox.ResetSkillCtrlHierarchy(this.gameObject);

        if (dropObjTarget != null) { dropObjTarget.JoinSkillBox(this); }
        else { NonSetBox?.JoinSkillBox(this); }
    }
}