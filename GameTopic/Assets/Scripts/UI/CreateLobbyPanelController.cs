using UnityEngine;
using TMPro;
using System;

public class CreateLobbyPanelController : MonoBehaviour
{
    public event Action<string> OnCreateLobby;
    [SerializeField]
    private TMP_InputField _lobbyNameInput;
    void Awake()
    {
        Debug.Assert(_lobbyNameInput != null);
    }
    public void OnPressCreateLobby(){
        OnCreateLobby?.Invoke(_lobbyNameInput.text);
    }
}