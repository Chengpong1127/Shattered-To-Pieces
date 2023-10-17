using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;

public class BaseConnectionManager: MonoBehaviour{
    [HideInInspector]
    public NetworkManager NetworkManager;
    /// <summary>
    /// The number of players in the game.
    /// </summary>
    public int PlayerCount = 1;

    public bool UseRelay = false;
    private UnityTransport RelayTransport;
    private UnityTransport UnityTransport;

    public event Action OnAllClientConnected;

    public virtual void StopConnection()
    {
        NetworkManager.Shutdown();
    }

    public virtual void StartConnection(NetworkType type, string ServerAddress){
        if (!string.IsNullOrEmpty(ServerAddress)){
            RelayTransport.ConnectionData.Address = ServerAddress;
            UnityTransport.ConnectionData.Address = ServerAddress;
        }
        NetworkManager.NetworkConfig.NetworkTransport = UseRelay ? RelayTransport : UnityTransport;
        switch(type){
            case NetworkType.Server:
                NetworkManager.StartServer();
                ServerWaitAllPlayerConnected();
                break;
            case NetworkType.Host:
                NetworkManager.StartHost();
                ServerWaitAllPlayerConnected();
                break;
            case NetworkType.Client:
                NetworkManager.StartClient();
                break;
        }
    }

    void Awake()
    {
        NetworkManager = FindObjectOfType<NetworkManager>();
        if(NetworkManager == null){
            var networkManagerPrefab = ResourceManager.Instance.LoadPrefab("NetworkManager");
            Instantiate(networkManagerPrefab);
            NetworkManager = FindObjectOfType<NetworkManager>();
            Debug.Log("Add NetworkManager at runtime");
        }
        var transports = NetworkManager.GetComponents<UnityTransport>();
        RelayTransport = transports.First(t => t.Protocol == UnityTransport.ProtocolType.RelayUnityTransport);
        UnityTransport = transports.First(t => t.Protocol == UnityTransport.ProtocolType.UnityTransport);
    }

    private async void ServerWaitAllPlayerConnected(){
        await UniTask.WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == PlayerCount);
        OnAllClientConnected?.Invoke();
        Debug.Log("ConnectionManager: All players connected");
    }


}