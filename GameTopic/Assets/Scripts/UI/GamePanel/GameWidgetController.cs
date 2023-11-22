using UnityEngine;

[RequireComponent(typeof(GameWidget))]
public class GameWidgetController : MonoBehaviour
{
    private GameWidget _gameWidget;
    public virtual void Show(){
        if(_gameWidget == null)
            _gameWidget = GetComponent<GameWidget>();
        _gameWidget.Show();
    }
    public virtual void Close(){
        if(_gameWidget == null)
            _gameWidget = GetComponent<GameWidget>();
        _gameWidget.Close();
    }
}