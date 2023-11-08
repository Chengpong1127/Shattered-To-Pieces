using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    public event Action<string> OnSendMessage;
    public StateMachine<GamePanelState> StateMachine;
    public CanvasGroup Background;
    public Transform PanelTransform;
    void Awake()
    {
        StateMachine = StateMachine<GamePanelState>.Initialize(this);
        StateMachine.ChangeState(GamePanelState.Hiding);
        PanelTransform.localScale = Vector3.zero;
        Background.alpha = 0;
        Background.interactable = false;
        Background.blocksRaycasts = false;
    }

    public async void EnterScene(){
        await EnterSceneAnimation();
        StateMachine.ChangeState(GamePanelState.Showing);
    }
    public async void ExitScene(){
        await ExitSceneAnimation();
        StateMachine.ChangeState(GamePanelState.Hiding);
    }
    protected virtual async UniTask EnterSceneAnimation(){
        await UniTask.WhenAll(
            DOTween.To(() => Background.alpha, x => Background.alpha = x, 1, 0.2f).ToUniTask(),
            PanelTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).ToUniTask()
        );
        Background.interactable = true;
        Background.blocksRaycasts = true;
    }
    protected virtual async UniTask ExitSceneAnimation(){
        await UniTask.WhenAll(
            DOTween.To(() => Background.alpha, x => Background.alpha = x, 0, 0.2f).ToUniTask(),
            PanelTransform.DOScale(0, 0.5f).SetEase(Ease.InBack).ToUniTask()
        );
        Background.interactable = false;
        Background.blocksRaycasts = false;
    }

    public void PanelSendMessage(string message){
        OnSendMessage?.Invoke(message);
    }


    public enum GamePanelState
    {
        Showing,
        Hiding
    }
}