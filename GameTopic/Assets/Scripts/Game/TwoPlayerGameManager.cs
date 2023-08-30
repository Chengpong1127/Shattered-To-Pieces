using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class TwoPlayerGameManager : MonoBehaviour {
    public LobbyManager lobbyManager;
    public Transform[] PlayerSpawnPoint;
    private PlayerDevice[] Players;
    async void Start()
    {
        await lobbyManager.SignIn();
        Debug.Assert(lobbyManager != null, "Lobby Manager is null");

        var lobby = await lobbyManager.GetTheLastestLobby();
        if(lobby != null){
            await lobbyManager.JoinLobby(lobby);
            string relayCode = lobbyManager.GetLobbyRelayCode();
            await lobbyManager.JoinRelay(relayCode, false);
            ClientStartGame();
        }else{
            await lobbyManager.CreateLobby("my lobby", 2);
            string relayCode = await lobbyManager.CreateRelay(2);
            Debug.Log("Relay Code: " + relayCode);
            await lobbyManager.AddRelayCodeAsync(relayCode);
            StartCoroutine(HostStartGame());
        }
    }

    IEnumerator HostStartGame(){
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host Start Game");
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == 2);
        Debug.Log("Player all connected");
        yield return new WaitForSeconds(3);
        DistributePlayers();
        new GameEffectManager();

    }
    void ClientStartGame(){
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client Start Game");
    }


    void DistributePlayers(){
        Players = FindObjectsOfType<PlayerDevice>();
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].SetRootPosition(PlayerSpawnPoint[i].position);
        }
    }
}