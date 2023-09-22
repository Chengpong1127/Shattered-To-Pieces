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
        draggerList.ForEach(d => {
            d.OwnerDropper = this;
        });
    }

    private void Start() {
        // RBDisplayer.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillDragger dragger = eventData.pointerDrag.GetComponent<SkillDragger>();
        if (dragger == null) { return; }
        dragger.Dropper = this;
    }

    public void AddSkill(int originBoxID, GameComponentAbility skillData) {
        Binder?.Bind(originBoxID, BoxID, skillData);
    }

    public void SetDisplay(List<GameComponentAbility> displayDatas) {
        int sid = 0;
        draggerList.ForEach(d => {
            d.UpdateDisplay(displayDatas != null && displayDatas.Count > sid ? displayDatas[sid] : null);
            sid++;
        });
    }
}
