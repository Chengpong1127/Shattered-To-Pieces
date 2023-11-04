using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameLoadingAnimation : BaseGameEventHandler
{
    public Animator LoadingAnimator;
    private async void Awake() {
        await UniTask.WaitUntil(() => IsClient || IsServer);
        if (IsClient)
        {
            Debug.Log("Start");
            LocalPlayerManager.StateMachine.Changed += OnManagerStateChanged;
        }
    }

    private void OnManagerStateChanged(LocalPlayerManager.LocalPlayerStates newState){
        if (newState == LocalPlayerManager.LocalPlayerStates.Gaming){
            LoadingAnimator.SetTrigger("End");
            Debug.Log("End");
        }
    }
}