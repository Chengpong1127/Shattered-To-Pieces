using UnityEngine;
using TMPro;
using System.Linq;
using Unity.Services.Lobbies.Models;
using Cysharp.Threading.Tasks;
using System;
using Unity.Services.Lobbies;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField]
    private Text _lobbyName;

    [SerializeField]
    private PlayerListController _playerListController;
    [SerializeField]
    private ReadyButtonController _readyButtonController;
    [SerializeField]
    private Text _playerCountText;
    [SerializeField]
    private MapInfoDisplay _mapInfoDisplay;
    private LobbyManager _lobbyManager;
    public event Action OnExitLobby;
    void Awake()
    {
        Debug.Assert(_lobbyName != null);
        Debug.Assert(_playerListController != null);
        Debug.Assert(_readyButtonController != null);
        Debug.Assert(_playerCountText != null);
        Debug.Assert(_mapInfoDisplay != null);

        _readyButtonController.SetReady();
        _readyButtonController.OnReadyButtonPressed += OnReadyButtonPressed;
    }
    public async void SetLobbyManager(LobbyManager lobbyManager){
        _lobbyManager = lobbyManager;
        await UniTask.WaitUntil(() => lobbyManager.CurrentLobby != null);
        _lobbyName.text = lobbyManager.CurrentLobby.Name;
        _playerCountText.text = GetPlayerCountText(lobbyManager.CurrentLobby);
        UpdatePlayerList();

        _lobbyManager.OnLobbyChanged += LobbyChangedHandler;
        string mapName = lobbyManager.CurrentLobby.Data["MapName"].Value;
        MapInfo mapInfo = ResourceManager.Instance.LoadMapInfo(mapName);
        _mapInfoDisplay.SetMapInfo(mapInfo);
    }

    public void ExitLobbyMode(){
        _lobbyManager.OnLobbyChanged -= LobbyChangedHandler;
        _lobbyManager = null;
    }

    private void LobbyChangedHandler(ILobbyChanges changed){
        if(changed.PlayerJoined.Changed || changed.PlayerLeft.Changed || changed.PlayerData.Changed)
            UpdatePlayerList();
        if(changed.LobbyDeleted && _lobbyManager.Identity != LobbyManager.LobbyIdentity.Host)
            OnExitLobby?.Invoke();
        _playerCountText.text = GetPlayerCountText(_lobbyManager.CurrentLobby);
    }

    public void ExitLobby_ButtonAction(){
        OnExitLobby?.Invoke();
    }

    private async void OnReadyButtonPressed(ReadyButtonController.ReadyButtonState state){
        switch(state){
            case ReadyButtonController.ReadyButtonState.Ready:
                _readyButtonController.SetUnready();
                await _lobbyManager.PlayerReady();
                UpdatePlayerList();
                break;
            case ReadyButtonController.ReadyButtonState.Unready:
                _readyButtonController.SetReady();
                await _lobbyManager.PlayerUnready();
                UpdatePlayerList();
                break;
        }
    }

    private void UpdatePlayerList(){
        var readyPlayers = _lobbyManager.CurrentLobby.Players.Where(player => player.Data["Ready"].Value == "true").ToList();
        var localPlayerIndex = _lobbyManager.CurrentLobby.Players.FindIndex(player => player.Id == _lobbyManager.SelfPlayer.Id);
        _playerListController.SetPlayerList(_lobbyManager.CurrentLobby.Players, readyPlayers, localPlayerIndex);
    }

    private string GetPlayerCountText(Lobby lobby){
        return lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
    }
}