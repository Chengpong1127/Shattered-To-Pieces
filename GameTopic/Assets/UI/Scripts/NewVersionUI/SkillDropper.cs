using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDropper : MonoBehaviour, IDropHandler {
    [SerializeField] public GameObject RBDisplayer;
    [SerializeField] public List<SkillDragger> draggerList;

    public SkillBinder Binder;
    public int BoxID { get; set; } = -1;

    private void Awake() {
        int i = 0;
        draggerList.ForEach(d => {
            d.OwnerDropper = this;
            d.draggerID = i;
            i++;
        });
    }

    private void Start() {
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillDragger dragger = eventData.pointerDrag.GetComponent<SkillDragger>();
        if (dragger == null) { return; }
        dragger.Dropper = this;
    }

    public void AddSkill(int originBoxID, int skillIndex) {
        Binder?.Bind(originBoxID, BoxID, skillIndex);
    }

    public void SetDisplay(List<DisplayableAbilityScriptableObject> displayDatas) {
        int sid = 0;
        draggerList.ForEach(d => {
            d.UpdateDisplay(displayDatas != null && displayDatas.Count > sid ? displayDatas[sid] : null);
            sid++;
        });
    }
    public void SetDisplay(int draggerID, DisplayableAbilityScriptableObject displayData) {
        if(draggerList.Count <= draggerID) { return; }
        draggerList[draggerID].UpdateDisplay(displayData);
    }
}
