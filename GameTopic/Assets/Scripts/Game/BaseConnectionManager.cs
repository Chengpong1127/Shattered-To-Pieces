using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using System.Linq;
using System;
using Cysharp.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

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

    public virtual void StartConnection(NetworkType type, NetworkIPPort ipPort){
        NetworkManager.NetworkConfig.NetworkTransport = UseRelay ? RelayTransport : UnityTransport;
        if(ipPort != null){
            RelayTransport.ConnectionData.Address = ipPort.IP;
            RelayTransport.ConnectionData.Port = ipPort.Port;

            UnityTransport.ConnectionData.Address = ipPort.IP;
            UnityTransport.ConnectionData.Port = ipPort.Port;
        }
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
        Debug.Log("Start as a host or a server. IP: " + GetLocalIPAddress());
        await UniTask.WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count == PlayerCount);
        OnAllClientConnected?.Invoke();
        Debug.Log("ConnectionManager: All players connected");
    }
    public string GetLocalIPAddress() {
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				return ip.ToString();
			}
		}
		throw new System.Exception("No network adapters with an IPv4 address in the system!");
	}

}