using UnityEngine;
using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class GameWidget : MonoBehaviour
{
    protected CanvasGroup CanvasGroup;
    [SerializeField]
    protected Ease EaseType = Ease.InOutBack;
    [SerializeField]
    protected float Duration = 0.5f;
    [SerializeField]
    protected bool DeactivateOnHide = true;

    public event Action<string> OnSendMessage;
    public StateMachine<GamePanelState> StateMachine;
    public bool IsShowing => StateMachine == null ? false : StateMachine.State == GamePanelState.Show;
    public bool IsClose => StateMachine == null ? true : StateMachine.State == GamePanelState.Close;
    protected virtual void Awake()
    {
        CanvasGroup = GetComponent<CanvasGroup>();
        StateMachine = StateMachine<GamePanelState>.Initialize(this);
        StateMachine.ChangeState(GamePanelState.Close);
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
    }

    public async void Show(){
        if(IsShowing) { return; }
        gameObject.SetActive(true);
        await ShowAnimation();
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
        StateMachine.ChangeState(GamePanelState.Show);
    }
    public async void Close(){
        if(IsClose) { return; }
        await CloseAnimation();
        CanvasGroup.interactable = false;
        CanvasGroup.blocksRaycasts = false;
        StateMachine.ChangeState(GamePanelState.Close);
        if(DeactivateOnHide)
            gameObject.SetActive(false);
    }
    protected virtual async UniTask ShowAnimation(){
        transform.localScale = Vector3.zero;
        await transform.DOScale(1, Duration).SetEase(EaseType).ToUniTask();
    }
    protected virtual async UniTask CloseAnimation(){
        transform.localScale = Vector3.one;
        await transform.DOScale(0, Duration).SetEase(EaseType).ToUniTask();
    }

    public void PanelSendMessage(string message){
        OnSendMessage?.Invoke(message);
    }


    public enum GamePanelState
    {
        Show,
        Close
    }
}