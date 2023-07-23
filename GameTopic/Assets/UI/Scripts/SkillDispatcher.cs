using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillDispatcher : MonoBehaviour
{
    [SerializeField] SkillBoxCtrl nonSetBox;
    [SerializeField] List<SkillBoxCtrl> skillBoxes;

    bool isDragging { get; set; }

    public bool isEditing { get; private set; }

    public UnityAction<int, Ability> setAbilityAction { get; set; }
    public UnityAction<Ability> setNullAbilityAction { get; set; }
    public UnityAction<int> refreshAbilityAction { get; set; }
    public UnityAction refreshNullAbilityAction { get; set; }
    public List<Ability> abilityList { get; set; }


    private void Awake() {
        abilityList = new List<Ability>();

        for (int i = 0; i < skillBoxes.Count; ++i) {
            skillBoxes[i].boxID = i;
        }

        skillBoxes.ForEach(box => {
            box.setAbilityAction += setAbilityAction;
            box.refreshAbilityAction += RefreshAbilityAction;
        });
        nonSetBox.setAbilityAction += SetNullAbilityAction;
        nonSetBox.refreshAbilityAction += RefreshNullAbilityAction;

        isEditing = true;
    }

    public void SetNullAbilityAction(int boxId, Ability ability) {
        setNullAbilityAction?.Invoke(ability);
    }
    public void RefreshAbilityAction(int boxId) {
        refreshAbilityAction?.Invoke(boxId);
        skillBoxes[boxId].SetSkillList(abilityList);
    }
    public void RefreshNullAbilityAction(int boxId) {
        refreshNullAbilityAction?.Invoke();
        nonSetBox.SetSkillList(abilityList);
    }
    public void RefreshAllBoxAbility() {
        skillBoxes.ForEach(box => {
            box.refreshAbilityAction(box.boxID);
        });
        nonSetBox.refreshAbilityAction(nonSetBox.boxID);
    }
    public void SwitchEditSkill() {
        isEditing = !isEditing;

        if (isEditing) {
            RefreshAllBoxAbility();
        }

        skillBoxes.ForEach(box => {
            box.SetActiveEditDisplayer(isEditing);
        });
        nonSetBox.gameObject.SetActive(isEditing);
    }
}
