using UnityEngine;
using TMPro;
using System.Linq;
using Unity.Services.Lobbies.Models;
using Cysharp.Threading.Tasks;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _lobbyName;

    [SerializeField]
    private PlayerListController _playerListController;
    [SerializeField]
    private ReadyButtonController _readyButtonController;
    private LobbyManager _lobbyManager;
    void Awake()
    {
        Debug.Assert(_lobbyName != null);
        Debug.Assert(_playerListController != null);
        Debug.Assert(_readyButtonController != null);

        _readyButtonController.OnReadyButtonPressed += OnReadyButtonPressed;
    }
    public async void SetLobbyManager(LobbyManager lobbyManager){
        _lobbyManager = lobbyManager;
        await UniTask.WaitUntil(() => lobbyManager.CurrentLobby != null);
        _lobbyName.text = lobbyManager.CurrentLobby.Name;
        UpdatePlayerList();

        _lobbyManager.OnLobbyChanged += changed => {
            if(changed.PlayerJoined.Changed || changed.PlayerLeft.Changed || changed.PlayerData.Changed)
                UpdatePlayerList();
        };
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
}