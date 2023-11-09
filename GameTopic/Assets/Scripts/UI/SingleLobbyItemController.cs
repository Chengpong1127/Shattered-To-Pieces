using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SingleLobbyItemController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _lobbyName;
    [SerializeField]
    private TMP_Text _lobbyPlayerCount;
    [SerializeField]
    private Button _joinButton;
    public event Action<string> OnPressJoin;
    private Lobby _lobby;
    void Awake()
    {
        _joinButton.onClick.AddListener(() => OnPressJoin?.Invoke(_lobby.Id));
    }

    public void SetLobby(Lobby lobby){
        _lobby = lobby;
        _lobbyName.text = lobby.Name;
        SetLobbyPlayerCount(lobby.Players.Count, lobby.MaxPlayers);
    }

    private void SetLobbyPlayerCount(int count,  int maxCount){
        _lobbyPlayerCount.text = count.ToString() + "/" + maxCount.ToString();
    }
}