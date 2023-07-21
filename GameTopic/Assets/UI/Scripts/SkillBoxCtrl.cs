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
    [SerializeField] int boxID;

    public UnityAction<int, Ability> SetAbilityAction;

    private void Start() {
        skillList.ForEach(skillCtrl => {
            skillCtrl.ownerBox = this;
        });
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillCtrl dragSkill = eventData.pointerDrag.GetComponent<SkillCtrl>();
        if (dragSkill == null) { return; }

        dragSkill.SetDropObjectTarget(this); // should be delete after use assemblyRoom to implement skill assign
        SetAbilityAction?.Invoke(boxID, dragSkill.skillData);
        // Debug.Log(gameObject.name + "get skill :" + dragSkill.skillData.AbilityName);
    }

    public void JoinSkillBox(SkillCtrl skill) {
        // skill.gameObject.transform.SetParent(skillCtrlDisplayer.transform, true);
        Debug.Log(gameObject.name + " get skill :" + skill.skillData.AbilityName);
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
            skillList[loopId].skillData = Ability.EmptyAbility();
            skillList[loopId].UpDateSkillDisplay();
        }
    }
}
 