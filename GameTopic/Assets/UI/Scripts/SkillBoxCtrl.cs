using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBoxCtrl : MonoBehaviour ,IDropHandler
{
    public void OnDrop(PointerEventData eventData) {
        GameObject dragObj = eventData.pointerDrag;
        if (dragObj == null) { return; }

        Debug.Log(eventData.pointerDrag.name);
        dragObj.transform.position = transform.position;
    }
}
