using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core;

public class SkillDropper : MonoBehaviour, IDropHandler {
    [SerializeField] public GameObject RBDisplayer;
    [SerializeField] public List<SkillDragger> draggerList;
    [SerializeField] private TMP_Text BindingKeyText;
    [SerializeField] public Button RebindBTN;
    [SerializeField] public Image RebindBTNImg;

    private TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> colorTween;

    public SkillBinder Binder;
    public int BoxID { get; set; } = -1;

    private void Awake() {
        int i = 0;
        draggerList.ForEach(d => {
            d.OwnerDropper = this;
            d.draggerID = i;
            i++;
        });
        BindingKeyText.text = "Non";
        GameEvents.RebindEvents.OnCancelRebinding += StopRebind;
        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += StartAbilityHandler;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += CancelAbilityHandler;
        
    }

    private async void StartAbilityHandler(int abilityID){
        if (abilityID == BoxID){
            await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.gray, 0.2f).ToUniTask();
        }
    }
    private async void CancelAbilityHandler(int abilityID){
        if (abilityID == BoxID){
            await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.white, 0.2f).ToUniTask();
        }
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
    public void SetDisplay(int draggerID, DisplayableAbilityScriptableObject displayData, GameComponent owner) {
        if(draggerList.Count <= draggerID) { return; }
        draggerList[draggerID].UpdateDisplay(displayData);
        draggerList[draggerID].SetOwner(owner);
    }

    public async void SetKeyText(string effectiveKey) {
        StopRebind();
        var displayKey = InputControlPath.ToHumanReadableString(effectiveKey, InputControlPath.HumanReadableStringOptions.OmitDevice);
        if (BindingKeyText.text != displayKey){
            BindingKeyText.text = displayKey;
            await SetKeyAnimation();
        }
        
    }
    private async UniTask SetKeyAnimation(){
        await UniTask.WhenAll(
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f).ToUniTask(),
            DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.green, 0.2f).ToUniTask()
        );
        await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.white, 0.2f).ToUniTask();
        
    }
    public async void StartRebind(){
        await UniTask.NextFrame();
        colorTween = DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.red, 0.5f)
            .SetLoops(-1, LoopType.Yoyo);
    }
    public void StopRebind(){
        colorTween?.Kill();
        RebindBTNImg.color = Color.white;
    }

    void OnDisable()
    {
        StopRebind();
    }

    void OnDestroy()
    {
        StopRebind();
    }
}
