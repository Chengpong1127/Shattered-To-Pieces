using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillBinder : MonoBehaviour {
    [SerializeField] Button EditBTN;

    [SerializeField] SkillDropper NonDropper;
    [SerializeField] List<SkillDropper> Droppers;

    public UnityAction<int, int, GameComponentAbility> setAbilityAction { get; set; }

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

    public void Bind(int origin, int newID, GameComponentAbility ability) {
        setAbilityAction?.Invoke(origin, newID, ability);
    }

    public void SetDisply(int id, List<GameComponentAbility> abilities) {
        if(id == -1) {
            NonDropper.SetDisplay(abilities);
        } else if (Droppers.Count > id) {
            Droppers[id].SetDisplay(abilities);
        }
    }
}
