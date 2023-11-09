using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System.Linq;
using System;

public class LobbyListController : MonoBehaviour
{
    public event Action<string> OnPlayerSelectLobby;
    [SerializeField]
    private GameObject LobbyItemPrefab;
    private List<SingleLobbyItemController> _lobbyItems = new();
    void Awake()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void SetLobbyList(List<Lobby> lobbies)
    {
        _lobbyItems.ForEach(lobbyItem => Destroy(lobbyItem.gameObject));
        lobbies.ForEach(lobby =>{
            var lobbyItem = Instantiate(LobbyItemPrefab, transform).GetComponent<SingleLobbyItemController>();
            lobbyItem.SetLobby(lobby);
            lobbyItem.OnPressJoin += OnPlayerSelectLobby;
            _lobbyItems.Add(lobbyItem);
        });

    }
}