using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillCtrl : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] RectTransform selfRectTransform;
    public void OnDrag(PointerEventData eventData) {
        Vector3 globalMouseePos;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(selfRectTransform, eventData.position,eventData.pressEventCamera, out globalMouseePos)) {
            selfRectTransform.position = globalMouseePos;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData) { } 

}