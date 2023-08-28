using Unity.Netcode;
using UnityEngine;
public class GameManager: SingletonMonoBehavior<GameManager>{
    public GameObject PlayerPrefab;
    public Transform[] PlayerSpawnPoint;
    public PlayerDevice[] Players;
    public void StartHost(){
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient(){
        NetworkManager.Singleton.StartClient();
    }
    public void StartServer(){
        NetworkManager.Singleton.StartServer();
    }
    void Start()
    {
        Players = FindObjectsOfType<PlayerDevice>();
        for (int i = 0; i < Players.Length; i++)
        {
            Players[i].SetPosition(PlayerSpawnPoint[i].position);
        }
    }
}