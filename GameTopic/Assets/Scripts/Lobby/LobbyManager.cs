using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using System.Net;
using Unity.Services.Lobbies.Models;

public class LobbyManager : MonoBehaviour
{
    private Lobby HostedLobby;
    void Start()
    {
        SignIn();
    }
    public async void SignIn(){
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    public async void CreateLobby(){
        string lobbyName = "My Lobby";
        int maxPlayers = 4;
        HostedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
        Debug.Log("Lobby Created " + HostedLobby.Name + " " + HostedLobby.LobbyCode);
    }
    public async void ListAllLobbies(){
        var responce = await LobbyService.Instance.QueryLobbiesAsync();
        Debug.Log("Lobby Count " + responce.Results.Count);
        foreach (var lobby in responce.Results)
        {
            Debug.Log("Lobby Name: " + lobby.Name + " Lobby Code: " + lobby.LobbyCode);
        }
    }

    public async void JoinLobby(){
        HostedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
        Debug.Log("Joined Lobby " + HostedLobby.Name);
    }
    public void ListLobbyPlayers(){
        if (HostedLobby == null)
        {
            Debug.Log("No Lobby Joined");
            return;
        }
        Debug.Log(HostedLobby.Players.Count);
        foreach (var player in HostedLobby.Players)
        {
            Debug.Log("Player Id: " + player.Id);
        }
    }

    public void StartGame(){
        Debug.Log("Starting Game with players:" + HostedLobby.Players.Count);
    }

}
