using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
public class PlayerSpawner{
    private GameObject playerPrefab;
    public PlayerSpawner(){
        playerPrefab = ResourceManager.Instance.LoadPlayerObject();
    }


    public Dictionary<ulong, IPlayer> SpawnAllPlayers()
    {
        Dictionary<ulong, IPlayer> Players = new();
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var player = SpawnPlayer(client.ClientId);
            Players.Add(client.ClientId, player.GetComponent<IPlayer>());
        }
        return Players;
    }

    private GameObject SpawnPlayer(ulong clientId)
    {
        var player = GameObject.Instantiate(playerPrefab);
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        return player;
    }
}