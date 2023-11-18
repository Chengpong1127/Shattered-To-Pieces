using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using System.Linq;
using System;
using TMPro;
using Cysharp.Threading.Tasks;

public class LobbyListController : MonoBehaviour
{
    public event Action<Lobby> OnPlayerSelectLobby;
    [SerializeField]
    private GameObject LobbyItemPrefab;
    [SerializeField]
    private Text NoLobbyText;
    [SerializeField]
    private GameObject LoadingObject;
    [SerializeField]
    private GameWidget GameWidget;
    private List<LobbyItemUIController> _lobbyItems = new();
    private List<ListItemAnimation> _listItemAnimations = new();
    void Awake()
    {
        Debug.Assert(LobbyItemPrefab != null);
        Debug.Assert(NoLobbyText != null);
        Debug.Assert(LoadingObject != null);
        Debug.Assert(GameWidget != null);
    }

    public void Show(){
        GameWidget.Show();
        _lobbyItems.ForEach(lobbyItem => Destroy(lobbyItem.gameObject));
        _listItemAnimations.Clear();
        _lobbyItems.Clear();
        NoLobbyText.enabled = false;
        LoadingObject.SetActive(true);
    }
    public void Close(){
        GameWidget.Close();
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
                var itemAnimation = lobbyItem.GetComponent<ListItemAnimation>();
                if (itemAnimation != null)
                {
                    _listItemAnimations.Add(itemAnimation);
                }
                lobbyItem.SetLobby(lobby);
                lobbyItem.OnPressJoin += OnPlayerSelectLobby;
                _lobbyItems.Add(lobbyItem);
                lobbyItem.gameObject.SetActive(false);
            });
            ShowListAnimation();
        }
    }
    private async void ShowListAnimation()
    {
        foreach (var item in _listItemAnimations)
        {
            item.gameObject.SetActive(true);
            item.ShowAnimation();
            await UniTask.WaitForSeconds(0.1f);
        }
    }
}