using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Transports.UTP;

public class GlobalConnectionManager : MonoBehaviour, INetworkConnector{
    public event Action OnAllPlayerConnected;
    public int AllPlayerCount = 1;
    public void StartConnection(){
        GlobalConnection();
    }

    private async void GlobalConnection(){
        var lobbyManager = new LobbyManager();
        await lobbyManager.SignIn();

        var lobby = await lobbyManager.GetTheLastestLobby();
        if(lobby != null){
            bool success = await lobbyManager.JoinLobby(lobby);
            if(success){
                return;
            }
        }
        await lobbyManager.CreateLobby("my lobby", AllPlayerCount);
        await WaitAllPlayerConnected();
    }

    private async UniTask WaitAllPlayerConnected(){
        await UniTask.WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == AllPlayerCount);
        OnAllPlayerConnected?.Invoke();
        Debug.Log("ConnectionManager: All players connected");
    }
}