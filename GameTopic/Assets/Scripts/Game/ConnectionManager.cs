using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class ConnectionManager : MonoBehaviour {
    private LobbyManager lobbyManager;
    public event Action OnAllPlayerConnected;
    public int AllPlayerCount;
    async void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) => {
            Debug.Log($"Client connected with id: {clientId}");
        };
        lobbyManager = new LobbyManager();
        await lobbyManager.SignIn();
        Debug.Assert(lobbyManager != null, "Lobby Manager is null");

        var lobby = await lobbyManager.GetTheLastestLobby();
        if(lobby != null){
            bool success = await lobbyManager.JoinLobby(lobby);
            if(success){
                Debug.Log("Joined Lobby");
                return;
            }
        }
        await lobbyManager.CreateLobby("my lobby", AllPlayerCount);
        StartCoroutine(WaitForAllConnection());
    }

    IEnumerator WaitForAllConnection(){
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == AllPlayerCount);
        OnAllPlayerConnected?.Invoke();
        Debug.Log("All Player Connected");
    }
}