using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;

public class GamePanel : MonoBehaviour
{
    public StateMachine<GamePanelState> StateMachine;
    void Awake()
    {
        StateMachine = StateMachine<GamePanelState>.Initialize(this);
        StateMachine.ChangeState(GamePanelState.Hiding);
        transform.localScale = Vector3.zero;
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
        await transform.DOScale(1, 0.5f).SetEase(Ease.OutBack).ToUniTask();
    }
    protected virtual async UniTask ExitSceneAnimation(){
        await transform.DOScale(0, 0.5f).SetEase(Ease.InBack).ToUniTask();
    }


    public enum GamePanelState
    {
        Showing,
        Hiding
    }
}