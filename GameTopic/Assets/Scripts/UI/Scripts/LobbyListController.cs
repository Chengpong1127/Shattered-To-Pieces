using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System.Linq;
using System;
using TMPro;

public class LobbyListController : MonoBehaviour
{
    public event Action<Lobby> OnPlayerSelectLobby;
    [SerializeField]
    private GameObject LobbyItemPrefab;
    [SerializeField]
    private TMP_Text NoLobbyText;
    [SerializeField]
    private GameObject LoadingObject;
    private List<LobbyItemUIController> _lobbyItems = new();
    void Awake()
    {
        Debug.Assert(LobbyItemPrefab != null);
        Debug.Assert(NoLobbyText != null);
        Debug.Assert(LoadingObject != null);
    }
    public void StartDisplay(){
        _lobbyItems.ForEach(lobbyItem => Destroy(lobbyItem.gameObject));
        _lobbyItems.Clear();
        NoLobbyText.enabled = false;
        LoadingObject.SetActive(true);
    }
    public void SetLobbyList(List<Lobby> lobbies)
    {
        LoadingObject.SetActive(false);
        if (lobbies.Count == 0)
        {
            NoLobbyText.enabled = true;
        }else{
            lobbies.ForEach(lobby =>{
                var lobbyItem = Instantiate(LobbyItemPrefab, transform).GetComponent<LobbyItemUIController>();
                lobbyItem.SetLobby(lobby);
                lobbyItem.OnPressJoin += OnPlayerSelectLobby;
                _lobbyItems.Add(lobbyItem);
            });
        }
    }
}