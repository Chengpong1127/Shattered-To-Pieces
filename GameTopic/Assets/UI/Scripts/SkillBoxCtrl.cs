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
    public UnityAction<int, Ability> setAbilityAction { get; set; }
    public UnityAction<int> refreshAbilityAction { get; set; }

    private void Awake() {
        skillList.ForEach(skillCtrl => {
            skillCtrl.ownerBox = this;
        });
    }

    private void Start() {
        // refreshAbilityAction?.Invoke(boxID);
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillCtrl dragSkill = eventData.pointerDrag.GetComponent<SkillCtrl>();
        if (dragSkill == null) { return; }

        dragSkill.SetDropObjectTarget(this);
    }


    public bool IsAssignedLastSkill() {
        return skillList[skillList.Count - 1].gameObject.activeSelf == true;
    }
    public void JoinSkillBox(Ability ability) {
        setAbilityAction?.Invoke(boxID, ability);
        refreshAbilityAction?.Invoke(boxID);
        Debug.Log(gameObject.name + " get skill : " + ability.AbilityName);
    }

    public void ResetSkillCtrlHierarchy(GameObject obj) {
        obj.transform.SetParent(skillCtrlDisplayer.transform, false);
        for(int i = 0; i < skillList.Count; ++i) {
            skillList[i].gameObject.transform.SetSiblingIndex(i);
        }

        refreshAbilityAction?.Invoke(boxID);
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

    public void SetActiveEditDisplayer(bool b) {
        skillCtrlDisplayer.SetActive(b);
        firstSkillDisplayImg.gameObject.SetActive(!b && firstSkillDisplayImg.sprite != null);
    }
}
 