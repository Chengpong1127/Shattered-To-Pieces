using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropper : MonoBehaviour, IDropHandler {
    [SerializeField] GameObject RBDisplayer;
    [SerializeField] List<SkillDragger> draggerList;
    

    private void Awake() {
        draggerList.ForEach(d => {
            d.OwnerDropper = this;
        });

        RBDisplayer.SetActive(false);
    }
    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillDragger dragger = eventData.pointerDrag.GetComponent<SkillDragger>();
        if (dragger == null) { return; }
        dragger.Dropper = this;
    }

    public void AddSkill(GameComponentAbility skillData) {
        Debug.Log("Add.");
    }

    public void RefreshDraggerDisplay() {
        Debug.Log("Refresh.");
    }
}
