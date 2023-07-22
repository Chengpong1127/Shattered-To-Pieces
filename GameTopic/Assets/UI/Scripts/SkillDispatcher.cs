using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillDispatcher : MonoBehaviour
{
    [SerializeField] SkillBoxCtrl nonSetBox;
    [SerializeField] List<SkillBoxCtrl> skillBoxes;

    public UnityAction<int, Ability> setAbilityAction { get; set; }
    public UnityAction<Ability> nonAbilityAction { get; set; }
    public List<Ability> abilityList { get; set; }

    private void Start() {
        for(int i=0;i < skillBoxes.Count; ++i) {
            skillBoxes[i].boxID = i;
        }

        abilityList = new List<Ability>();

        SetAbilityAction(UpDateAbilityFunction);
        nonSetBox.SetAbilityAction += UpDateNonAbilityFunction;
    }

    public void SetAbilityAction(UnityAction<int, Ability> action) {
        skillBoxes.ForEach(box => {
            box.SetAbilityAction += action;
        });
    }

    public void UpDateAbilityFunction(int boxId, Ability ability) {
        setAbilityAction?.Invoke(boxId, ability);
        skillBoxes[boxId].SetSkillList(abilityList);
    }
    public void UpDateNonAbilityFunction(int boxId,Ability ability) {
        nonAbilityAction?.Invoke(ability);
        nonSetBox.SetSkillList(abilityList);
    }
}
