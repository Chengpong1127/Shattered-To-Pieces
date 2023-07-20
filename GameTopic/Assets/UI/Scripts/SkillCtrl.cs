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

    RectTransform selfRectTransform;
    Image selfImage;
    SkillBoxCtrl dropObjTarget;

    SkillData skillData;

    private void Awake() {
        selfRectTransform  = GetComponent<RectTransform>();
        selfImage = GetComponent<Image>();
        dropObjTarget = null;

        if (selfImage == null) {
            Debug.Log("image not found.");
            gameObject.SetActive(false);
        }

        NonSetBox = GameObject.Find("NonSetBox").GetComponent<SkillBoxCtrl>();
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

        if(dropObjTarget != null) { dropObjTarget.JoinSkillBox(this); }
        else { NonSetBox?.JoinSkillBox(this); }
    }
}