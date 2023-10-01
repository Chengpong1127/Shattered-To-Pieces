using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillDispatcher : MonoBehaviour
{
    [SerializeField] SkillBoxCtrl nonSetBox;
    [SerializeField] List<SkillBoxCtrl> skillBoxes;

    // bool isDragging { get; set; }

    public bool isEditing { get; private set; }

    // reassign skill to other box
    public UnityAction<int, GameComponentAbility> setAbilityAction { get; set; }
    public UnityAction<GameComponentAbility> setNullAbilityAction { get; set; }
    public UnityAction<int> refreshAbilityAction { get; set; }
    public UnityAction refreshNullAbilityAction { get; set; }
    public List<GameComponentAbility> abilityList { get; set; }

    // rebind box key
    public UnityAction<int> rebindKeyAction { get; set; }
    public int rebindBoxId { get; set; }

    private void Awake() {
        // abilityList = new List<Ability>();

        nonSetBox.boxID = 0;
        for (int i = 0; i < skillBoxes.Count; ++i) {
            skillBoxes[i].boxID = i;
        }

        skillBoxes.ForEach(box => {
            box.setAbilityAction += SetAbilityAction;
            box.refreshAbilityAction += RefreshAbilityAction;
            box.rebindKeyAction += RebindKeyAction;
        });
        nonSetBox.setAbilityAction += SetNullAbilityAction;
        nonSetBox.refreshAbilityAction += RefreshNullAbilityAction;

        isEditing = true;
    }

    /// <summary>
    /// An invoke function that set ability in a specified entry.
    /// </summary>
    /// <param name="boxId"></param>
    /// <param name="ability"></param>
    public void SetAbilityAction(int boxId, GameComponentAbility ability) {
        bool refreshNullAbility = skillBoxes[boxId].IsAssignedLastSkill();
        setAbilityAction?.Invoke(boxId, ability);
        if (refreshNullAbility) {
            RefreshNullAbilityAction(0);
        }
    }

    /// <summary>
    /// An invoke function that set ability in non assigned entry.
    /// </summary>
    /// <param name="boxId"></param>
    /// <param name="ability"></param>
    public void SetNullAbilityAction(int boxId, GameComponentAbility ability) {
        setNullAbilityAction?.Invoke(ability);
    }

    /// <summary>
    /// An invoke function that refresh the certain entry UI.
    /// </summary>
    /// <param name="boxId"></param>
    public void RefreshAbilityAction(int boxId) {
        refreshAbilityAction?.Invoke(boxId);
        skillBoxes[boxId].SetSkillList(abilityList);
    }

    /// <summary>
    /// An invoke function that refresh the non assigned entry UI.
    /// </summary>
    /// <param name="boxId"></param>
    public void RefreshNullAbilityAction(int boxId) {
        refreshNullAbilityAction?.Invoke();
        nonSetBox.SetSkillList(abilityList);
    }

    /// <summary>
    /// Update all entry UI.
    /// </summary>
    public void RefreshAllBoxAbility() {
        skillBoxes.ForEach(box => {
            box.refreshAbilityAction(box.boxID);
        });
        nonSetBox.refreshAbilityAction(nonSetBox.boxID);
    }

    /// <summary>
    /// Open or close skill editing mode.
    /// </summary>
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

    /// <summary>
    /// An invoke function to execute rebind a entry key.
    /// </summary>
    /// <param name="boxId"></param>
    public void RebindKeyAction(int boxId) {
        rebindBoxId = boxId;
        rebindKeyAction?.Invoke(boxId);
    }

    /// <summary>
    /// Set text into certain entry UI for binding key.
    /// </summary>
    /// <param name="keyText"></param>
    public void SetRebindKeyText(string keyText) {
        if(rebindBoxId >= skillBoxes.Count) { return; }
        // Debug.Log("call SetRebindKeyText : " + keyText);
        skillBoxes[rebindBoxId].SetBindKeyText(keyText);
    }
}
