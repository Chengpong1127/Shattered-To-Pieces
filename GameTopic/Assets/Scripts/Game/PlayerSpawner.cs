using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
public class PlayerSpawner{
    private GameObject playerPrefab;
    public Dictionary<ulong, GameObject> Players = new();
    public PlayerSpawner(){
        playerPrefab = ResourceManager.Instance.LoadPlayerObject();
    }


    public void SpawnAllPlayers()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = SpawnPlayer(client.ClientId);
            Players.Add(client.ClientId, player);
        }
    }

    private GameObject SpawnPlayer(ulong clientId)
    {
        var player = GameObject.Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        return player;
    }
}