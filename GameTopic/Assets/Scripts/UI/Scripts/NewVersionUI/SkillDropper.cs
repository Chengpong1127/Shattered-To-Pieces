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
using System;

public class SkillDropper : MonoBehaviour, IDropHandler {
    [SerializeField] public List<SkillDragger> draggerList;
    [SerializeField] private TMP_Text BindingKeyText;
    [SerializeField] private Image RebindBTNImg;
    [SerializeField] private Button button;
    public event Action<int> OnRebindStart;
    public event Action<int, int, int> OnAddNewSkill;

    private TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> colorTween;
    public int SelfBoxID { get; set; } = -1;
    public bool IsShowing => RebindBTNImg.raycastTarget;

    private void Awake() {
        Debug.Assert(draggerList != null);
        Debug.Assert(BindingKeyText != null);
        Debug.Assert(RebindBTNImg != null);
        Debug.Assert(button != null);

        draggerList.ForEach(d => {
            d.OwnerDropper = this;
            d.draggerID = draggerList.IndexOf(d);
        });
        button.onClick.AddListener(RebindKeyHandler_ButtonAction);
        BindingKeyText.text = "Non";
        GameEvents.RebindEvents.OnCancelRebinding += StopRebind;
        GameEvents.AbilityRunnerEvents.OnLocalInputStartAbility += StartAbilityHandler;
        GameEvents.AbilityRunnerEvents.OnLocalInputCancelAbility += CancelAbilityHandler;

        transform.localScale = Vector3.zero;
    }

    public async UniTask InitializeAnimation(){
        transform.localScale = Vector3.one * 0.5f;
        RebindBTNImg.color = new Color(RebindBTNImg.color.r, RebindBTNImg.color.g, RebindBTNImg.color.b, 0.5f);
        await UniTask.WhenAll(
            transform.DOScale(Vector3.one, 0.2f).ToUniTask(),
            DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.white, 0.2f).ToUniTask()
        );
    }

    public async void Show(){
        RebindBTNImg.raycastTarget = true;
        button.interactable = true;
        draggerList.ForEach(d => d.Show());
        await DOTween.To(() => RebindBTNImg.color.a, x => RebindBTNImg.color = new Color(RebindBTNImg.color.r, RebindBTNImg.color.g, RebindBTNImg.color.b, x), 1f, 0.2f).ToUniTask();
    }
    public async void Hide(){
        RebindBTNImg.raycastTarget = false;
        button.interactable = false;
        draggerList.ForEach(d => d.Hide());
        await DOTween.To(() => RebindBTNImg.color.a, x => RebindBTNImg.color = new Color(RebindBTNImg.color.r, RebindBTNImg.color.g, RebindBTNImg.color.b, x), 0.5f, 0.2f).ToUniTask();
    }

    private async void StartAbilityHandler(int abilityID){
        if (abilityID == SelfBoxID){
            if (IsShowing){
                await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.gray, 0.2f).ToUniTask();
            }
            else{
                await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.5f), 0.2f).ToUniTask();
            }
        }
    }
    private async void CancelAbilityHandler(int abilityID){
        if (abilityID == SelfBoxID){
            if (IsShowing){
                await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.white, 0.2f).ToUniTask();
            }
            else{
                await DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f), 0.2f).ToUniTask();
            }
        }
    }

    public void OnDrop(PointerEventData eventData) {
        if (eventData.pointerDrag == null) { return; }
        SkillDragger dragger = eventData.pointerDrag.GetComponent<SkillDragger>();
        if (dragger == null) { return; }
        dragger.Dropper = this;
    }

    public void AddSkill(int originBoxID, int skillIndex) {
        OnAddNewSkill?.Invoke(originBoxID, SelfBoxID, skillIndex);
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
    public void RebindKeyHandler_ButtonAction(){
        OnRebindStart?.Invoke(SelfBoxID);
        StartRebind();
    }
    public async void StartRebind(){
        await UniTask.NextFrame();
        colorTween = DOTween.To(() => RebindBTNImg.color, x => RebindBTNImg.color = x, Color.red, 0.5f)
            .SetLoops(-1, LoopType.Yoyo);
    }
    public void StopRebind(){
        if (colorTween != null){
            colorTween.Kill();
            RebindBTNImg.color = Color.white;
        }
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
