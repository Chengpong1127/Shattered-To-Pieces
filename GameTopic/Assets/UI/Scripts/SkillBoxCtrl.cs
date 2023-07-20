using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct SkillData {

}


public class SkillBoxCtrl : MonoBehaviour ,IDropHandler
{
    [SerializeField] Image firstSkillDisplayImg;
    [SerializeField] GameObject skillCtrlDisplayer;

    List<SkillCtrl> skillList;
    public void OnDrop(PointerEventData eventData) {
        GameObject dragObj = eventData.pointerDrag;
        if (dragObj == null) { return; }

        dragObj.GetComponent<SkillCtrl>().SetDropObjectTarget(this);
        dragObj.transform.SetParent(skillCtrlDisplayer.transform, true);

        // Debug.Log(eventData.pointerDrag.name);
        // dragObj.transform.position = transform.position;
    }
}
