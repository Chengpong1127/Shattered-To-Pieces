using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBoxCtrl : MonoBehaviour ,IDropHandler
{
    [SerializeField] Image firstSkillDisplayImg;
    [SerializeField] GameObject skillCtrlDisplayer;
    [SerializeField] List<SkillCtrl> skillList;
    public int boxID {  get; set; }
    public UnityAction<int, Ability> SetAbilityAction { get; set; }

    private void Start() {
        skillList.ForEach(skillCtrl => {
            skillCtrl.ownerBox = this;
        });
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillCtrl dragSkill = eventData.pointerDrag.GetComponent<SkillCtrl>();
        if (dragSkill == null) { return; }

        dragSkill.SetDropObjectTarget(this);
    }

    public void JoinSkillBox(SkillCtrl skill) {
        SetAbilityAction?.Invoke(boxID, skill.skillData);
        Debug.Log(gameObject.name + " get skill");
    }

    public void ResetSkillCtrlHierarchy(GameObject obj) {
        obj.transform.SetParent(skillCtrlDisplayer.transform, false);
    }

    public void SetSkillList(List<Ability> skills) {
        int loopId = 0;

        for(; loopId < skillList.Count && loopId < skills.Count;loopId++) {
            skillList[loopId].skillData = skills[loopId];
            skillList[loopId].UpDateSkillDisplay();
        }
        for(; loopId < skillList.Count;loopId++ ) {
            skillList[loopId].skillData = null;
            skillList[loopId].UpDateSkillDisplay();
        }
    }
}
 