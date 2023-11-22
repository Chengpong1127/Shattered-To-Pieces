using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(GameWidget))]
public class GameWidgetController : MonoBehaviour
{
    private GameWidget _gameWidget;
    public event Action OnShow;
    public event Action OnClose;
    public virtual void Show(){
        if(_gameWidget == null)
            SetGameWidget();
        _gameWidget.Show();
    }
    public virtual void Close(){
        if(_gameWidget == null)
            SetGameWidget();
        _gameWidget.Close();
    }
    private async void SetGameWidget(){
        _gameWidget = GetComponent<GameWidget>();
        await UniTask.WaitUntil(() => _gameWidget.StateMachine != null);
        _gameWidget.StateMachine.Changed += GameWidgetStateMachineChangedHandler;
    }
    private void GameWidgetStateMachineChangedHandler(GameWidget.GameWidgetState state){
        switch(state){
            case GameWidget.GameWidgetState.Show:
                OnShow?.Invoke();
                break;
            case GameWidget.GameWidgetState.Close:
                OnClose?.Invoke();
                break;
        }
    }
}