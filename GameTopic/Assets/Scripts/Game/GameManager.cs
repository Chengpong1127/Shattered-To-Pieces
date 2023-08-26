using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using NetworkEvent = Unity.Networking.Transport.NetworkEvent;

public class GameManager: MonoBehaviour{
    const int m_MaxConnections = 4;

	public string RelayJoinCode;
    public void StartHost(){
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient(){
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer(){
        NetworkManager.Singleton.StartServer();
    }
    async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    private async void CreateRelay(){
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("Join code: " + joinCode);
        RelayServerData data = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        NetworkManager.Singleton.StartHost();

    }
    private async void JoinRelay(string joinCode){
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        RelayServerData data = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(data);
        NetworkManager.Singleton.StartClient();


    }
}