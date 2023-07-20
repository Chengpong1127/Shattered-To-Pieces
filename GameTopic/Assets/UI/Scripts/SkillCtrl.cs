using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCtrl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Transform canvasTransform;

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

        transform.SetParent(canvasTransform, false);
    }

    public void OnEndDrag(PointerEventData eventData) {
        selfImage.raycastTarget = true;
        if(dropObjTarget != null) {
            // gameObject.transform.SetParent(dropObjTarget.transform.transform);
            // selfRectTransform.anchoredPosition = dropObjTarget.GetComponent<RectTransform>().anchoredPosition;
        }
    }

}