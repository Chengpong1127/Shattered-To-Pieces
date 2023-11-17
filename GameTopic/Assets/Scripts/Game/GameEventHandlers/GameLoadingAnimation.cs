using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameLoadingAnimation : BaseGameEventHandler
{
    public Animator LoadingAnimator;
    private async void Awake() {
        await UniTask.WaitUntil(() => IsClient || IsServer);
        if (IsClient)
        {
            LocalPlayerManager.StateMachine.Changed += OnManagerStateChanged;
        }
    }
    public override void OnDestroy() {
        if (IsClient)
        {
            LocalPlayerManager.StateMachine.Changed -= OnManagerStateChanged;
        }
        base.OnDestroy();
    }

    private void OnManagerStateChanged(LocalPlayerManager.LocalPlayerStates newState){
        if (newState == LocalPlayerManager.LocalPlayerStates.Gaming){
            LoadingAnimator.SetTrigger("End");
        }
    }
}