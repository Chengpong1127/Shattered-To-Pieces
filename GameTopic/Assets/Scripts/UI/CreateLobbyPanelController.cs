using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class CreateLobbyPanelController : MonoBehaviour
{
    public event Action<string> OnCreateLobby;
    [SerializeField]
    private TMP_InputField _lobbyNameInput;
    [SerializeField]
    private GameWidget _gameWidget;
    [SerializeField]
    private RainbowText _rainbowText;
    void Awake()
    {
        Debug.Assert(_lobbyNameInput != null);
        Debug.Assert(_gameWidget != null);
        Debug.Assert(_rainbowText != null);
    }


    public void Show(){
        _gameWidget.Show();
        _lobbyNameInput.text = "";
        _rainbowText.StarRainbow();
    }
    public void Close(){
        _gameWidget.Close();
    }
    public void OnPressCreateLobby(){
        OnCreateLobby?.Invoke(_lobbyNameInput.text);
    }

}