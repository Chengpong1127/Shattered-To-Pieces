using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillBinder : MonoBehaviour {
    [SerializeField] Button EditBTN;

    [SerializeField] SkillDropper NonDropper;
    [SerializeField] List<SkillDropper> Droppers;

    public UnityAction<int, int, int> setAbilityAction { get; set; }

    private void Awake() {
        NonDropper.Binder = this;
        NonDropper.draggerList.ForEach(d => {
            d.NonSetDropper = NonDropper;
            d.DraggingParentTransform = this.transform.parent;
        });
        int id = 0;
        Droppers.ForEach(d => {
            d.draggerList.ForEach(d => {
                d.NonSetDropper = NonDropper;
                d.DraggingParentTransform = this.transform.parent;
            });
            d.Binder = this;
            d.BoxID = id;
            id++;
        });
    }

    public void Bind(int origin, int newID, int abilityID) {
        setAbilityAction?.Invoke(origin, newID, abilityID);
    }

    public void SetDisply(int id, List<DisplayableAbilityScriptableObject> abilities) {
        if(id == -1) {
            NonDropper.SetDisplay(abilities);
        } else if (Droppers.Count > id) {
            Droppers[id].SetDisplay(abilities);
        }
    }

    public void SetDisply(int boxID, int abilityID, DisplayableAbilityScriptableObject DASO) {
        if (boxID == -1) { NonDropper.SetDisplay(abilityID, DASO); }
        else { Droppers[boxID].SetDisplay(abilityID, DASO); }
    }
}
