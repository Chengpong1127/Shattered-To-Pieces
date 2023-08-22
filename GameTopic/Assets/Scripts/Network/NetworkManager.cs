using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class NetworkManager : SingletonMonoBehavior<NetworkManager>, INetworkRunnerCallbacks
{
    public NetworkRunner Runner;
    public NetworkPrefabRef networkPrefabRef;
    public Dictionary<PlayerRef, Device> PlayerDeviceMap = new();
    private void Start() {
        StartGame(GameMode.AutoHostOrClient);
    }

    async void StartGame(GameMode mode){
        var args = new StartGameArgs{
            GameMode = mode,
            SessionName = "MySession",
            Scene = SceneManager.GetActiveScene().buildIndex,
        };
        await Runner.StartGame(args);
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (Runner.IsClient) return;
        var factory = new NetworkGameComponentFactory(runner, player);
        PlayerDeviceMap[player] = new Device(factory);
        var deviceInfo = ResourceManager.Instance.LoadLocalDeviceInfo("0");
        PlayerDeviceMap[player].Load(deviceInfo ?? ResourceManager.Instance.LoadDefaultDeviceInfo());
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //throw new NotImplementedException();
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        //throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        //throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        //throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        //throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        //throw new NotImplementedException();
    }



    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        //throw new NotImplementedException();
    }
}