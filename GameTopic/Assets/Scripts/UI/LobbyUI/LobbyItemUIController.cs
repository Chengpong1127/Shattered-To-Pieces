using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class LobbyItemUIController : MonoBehaviour
{
    [SerializeField]
    private Text _lobbyName;
    [SerializeField]
    private Text _lobbyPlayerCount;
    [SerializeField]
    private Button _joinButton;
    public event Action<Lobby> OnPressJoin;
    private Lobby _lobby;
    void Awake()
    {
        Debug.Assert(_lobbyName != null);
        Debug.Assert(_lobbyPlayerCount != null);
        Debug.Assert(_joinButton != null);
    }

    public void SetLobby(Lobby lobby){
        _lobby = lobby;
        _lobbyName.text = lobby.Name;
        SetLobbyPlayerCount(lobby.Players.Count, lobby.MaxPlayers);
    }

    private void SetLobbyPlayerCount(int count,  int maxCount){
        _lobbyPlayerCount.text = count.ToString() + "/" + maxCount.ToString();
    }

    public void JoinLobby_ButtonAction(){
        OnPressJoin?.Invoke(_lobby);
    }
}