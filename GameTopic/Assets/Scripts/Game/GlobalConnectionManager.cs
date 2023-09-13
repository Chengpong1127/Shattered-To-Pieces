using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using System.Linq;

public class GlobalConnectionManager : MonoBehaviour, INetworkConnector{
    private UnityTransport RelayTransport;
    private UnityTransport LocalTransport;
    private void Awake(){
        var transports = NetworkManager.Singleton.GetComponents<UnityTransport>();
        RelayTransport = transports.First(t => t.Protocol == UnityTransport.ProtocolType.RelayUnityTransport);
        LocalTransport = transports.First(t => t.Protocol == UnityTransport.ProtocolType.UnityTransport);
    }
    public event Action OnAllPlayerConnected;
    public int AllPlayerCount = 1;
    public void StartConnection(){
        if(AllPlayerCount == 1){
            SingleConnection();
        }else{
            GlobalConnection();
        }
    }

    private async void GlobalConnection(){
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = RelayTransport;
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

    private async void SingleConnection(){
        NetworkManager.Singleton.NetworkConfig.NetworkTransport = LocalTransport;
        NetworkManager.Singleton.StartHost();
        await WaitAllPlayerConnected();
    }

    private async UniTask WaitAllPlayerConnected(){
        await UniTask.WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == AllPlayerCount);
        OnAllPlayerConnected?.Invoke();
        Debug.Log("ConnectionManager: All players connected");
    }
}