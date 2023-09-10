
using UnityEngine;
using System;
using Unity.Netcode;

public class SingleConnectionManager : MonoBehaviour, INetworkConnector
{
    public event Action OnAllPlayerConnected;

    public void StartConnection()
    {
        NetworkManager.Singleton.StartHost();
        OnAllPlayerConnected?.Invoke();
    }
}