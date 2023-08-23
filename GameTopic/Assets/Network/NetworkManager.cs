using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;
using Newtonsoft.Json;
using Unity.Collections;
using System.Text;

public class NetworkManager : SingletonMonoBehavior<NetworkManager>, INetworkRunnerCallbacks
{
    private NetworkRunner Runner;
    public NetworkPrefabRef PlayerDevice;
    private void Start() {
        Application.targetFrameRate = 60;
        Runner = gameObject.AddComponent<NetworkRunner>();
        StartGame(GameMode.AutoHostOrClient);
    }

    async void StartGame(GameMode mode){
        Runner.ProvideInput = true;
        var args = new StartGameArgs{
            GameMode = mode,
            SessionName = "MySession",
            Scene = SceneManager.GetActiveScene().buildIndex,
        };
        await Runner.StartGame(args);
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player joined: " + player);
        if (Runner.IsServer){
            Runner.Spawn(PlayerDevice, inputAuthority: player);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player left: " + player);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connect failed: " + reason);
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("Connect request: " + request);
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log("Custom auth response: " + data);
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log("Host migration: " + hostMigrationToken);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("Input missing: " + input);
    }



    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log("Reliable data received: " + data);
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("Scene load done");
        if(runner.IsServer){
            //factory = runner.Spawn(FactoryPrefab).GetComponent<NetworkGameComponentFactory>();
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Scene load start");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("Session list updated: " + sessionList);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Shutdown: " + shutdownReason);
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("User simulation message: " + message);
    }
}