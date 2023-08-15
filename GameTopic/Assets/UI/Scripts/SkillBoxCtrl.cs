using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] TMP_Text bindKeyDisplay;
    [SerializeField] List<SkillCtrl> skillList;
    public int boxID {  get; set; }
    public UnityAction<int, GameComponentAbility> setAbilityAction { get; set; }
    public UnityAction<int> refreshAbilityAction { get; set; }
    public UnityAction<int> rebindKeyAction { get; set; }

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

    /// <summary>
    /// Check is the last skillCtrl have data or not.
    /// It means the current entry is full.
    /// </summary>
    /// <returns></returns>
    public bool IsAssignedLastSkill() {
        return skillList[skillList.Count - 1].gameObject.activeSelf == true;
    }

    /// <summary>
    /// Set ability to certain skill box.
    /// </summary>
    /// <param name="ability"></param>
    public void JoinSkillBox(GameComponentAbility ability) {
        setAbilityAction?.Invoke(boxID, ability);
        refreshAbilityAction?.Invoke(boxID);
        // Debug.Log(gameObject.name + " get skill : " + ability.AbilityName);
    }

    /// <summary>
    /// Set gameobject's hierarchy under skillCtrl displayer and refresh ability display.
    /// It'll make sure the sibling under skillCtrlDisplayer.
    /// </summary>
    /// <param name="obj"></param>
    public void ResetSkillCtrlHierarchy(GameObject obj) {
        obj.transform.SetParent(skillCtrlDisplayer.transform, false);
        for(int i = 0; i < skillList.Count; ++i) {
            skillList[i].gameObject.transform.SetSiblingIndex(i);
        }

        refreshAbilityAction?.Invoke(boxID);
    }

    /// <summary>
    /// Write abilitys into each skillCttrl's skillData and update them.
    /// </summary>
    /// <param name="skills"></param>
    public void SetSkillList(List<GameComponentAbility> skills) {
        int loopId = 0;
        if(skills == null) {
            for (; loopId < skillList.Count; loopId++) {
                skillList[loopId].skillData = null;
                skillList[loopId].UpDateSkillDisplay();
            }
        }

        for(; loopId < skillList.Count && loopId < skills.Count;loopId++) {
            skillList[loopId].skillData = skills[loopId];
            skillList[loopId].UpDateSkillDisplay();
        }
        for(; loopId < skillList.Count;loopId++ ) {
            skillList[loopId].skillData = null;
            skillList[loopId].UpDateSkillDisplay();
        }
    }

    /// <summary>
    /// Open or close the skill edit displayer, rebind text button and skill display image.
    /// </summary>
    /// <param name="b"></param>
    public void SetActiveEditDisplayer(bool b) {
        skillCtrlDisplayer.SetActive(b);
        bindKeyDisplay.gameObject.SetActive(b);
        firstSkillDisplayImg.gameObject.SetActive(!b && firstSkillDisplayImg.sprite != null);
    }

    /// <summary>
    /// Invoke function for rebind key button click.
    /// </summary>
    public void OnClickRebindKey() {
        rebindKeyAction?.Invoke(boxID); // should work when rebind function is implemented.
        // Debug.Log("Rebind keys not yet implemented.");
    }

    /// <summary>
    /// Set binded key text to text UI.
    /// </summary>
    /// <param name="keyText"></param>
    public void SetBindKeyText(string keyText) {
        bindKeyDisplay.text = keyText;
    }
}
 