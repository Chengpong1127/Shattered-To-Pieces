using UnityEngine;
using Cysharp.Threading.Tasks;
using MonsterLove.StateMachine;
using System;

public abstract class BaseGamePanel : MonoBehaviour
{
    public event Action<string> OnSendMessage;
    public StateMachine<GamePanelState> StateMachine;
    protected virtual void Awake()
    {
        StateMachine = StateMachine<GamePanelState>.Initialize(this);
        StateMachine.ChangeState(GamePanelState.Hiding);
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
        await UniTask.Yield();
    }
    protected virtual async UniTask ExitSceneAnimation(){
        await UniTask.Yield();
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