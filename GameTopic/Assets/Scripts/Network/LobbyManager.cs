using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

public class LobbyManager
{
    // Server Events
    public event Action<Player> OnPlayerJoined;
    public event Action<Player> OnPlayerReady;
    public event Action<Player> OnPlayerUnready;

    // Client Events
    public event Action OnLobbyReady;


    public Lobby CurrentLobby { get; private set; }
    public Player SelfPlayer { get; private set; }

    public LobbyManager(Player player){
        SelfPlayer = player;
    }
    public async UniTask<Lobby> CreateLobby(string lobbyName, int maxPlayers){
        var createLobbyOptions = new CreateLobbyOptions(){
            IsPrivate = false,
            IsLocked = false,
            Player = SelfPlayer};
        var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        BindHostLobbyHandler(lobby);

        await UpdateLobbyDataAsync(lobby, "Ready", "false");
        await UpdateSelfPlayerDataAsync(lobby, SelfPlayer, "Ready", "false");
        return lobby;
    }

    public async UniTask<Lobby[]> GetAllAvailableLobby(){
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
            Order = new List<QueryOrder> {
                new QueryOrder (field: QueryOrder.FieldOptions.Created, asc: true)
            },
            Filters = new List<QueryFilter>{
                new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GE, value: "1")
            }
        };
        var responce = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
        return responce.Results.ToArray();
    }

    public async UniTask JoinLobby(Lobby lobby){
        JoinLobbyByIdOptions quickJoinLobbyOptions = new JoinLobbyByIdOptions { Player = SelfPlayer };
        await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, quickJoinLobbyOptions);
        BindClinetLobbyHandler(lobby);

        await UpdateSelfPlayerDataAsync(lobby, SelfPlayer, "Ready", "false");
    }

    private void BindHostLobbyHandler(Lobby lobby){
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.DataChanged += DataChangedHandler;
        lobbyEventCallbacks.PlayerJoined += player => OnPlayerJoined?.Invoke(player.First().Player);
        lobbyEventCallbacks.PlayerDataChanged += data => HostPlayerDataChangedHandler(lobby, data);
        LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }

    private void HostPlayerDataChangedHandler(Lobby lobby, Dictionary<int, Dictionary<string, ChangedOrRemovedLobbyValue<PlayerDataObject>>> data)
    {
        var readyPlayers = data.Where(playerData => playerData.Value["Ready"].Value.Value == "true")
                                .Select(playerData => lobby.Players[playerData.Key]);

        readyPlayers.ToList().ForEach(player => OnPlayerReady?.Invoke(player));
    }

    private void BindClinetLobbyHandler(Lobby lobby){
        LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
        lobbyEventCallbacks.DataChanged += DataChangedHandler;
        LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, lobbyEventCallbacks);
    }

    private void DataChangedHandler(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> data){
        if (data["Ready"].Value.Value == "true"){
            OnLobbyReady?.Invoke();
        }
    }

    public async void PlayerReady(){
        await UpdateSelfPlayerDataAsync(CurrentLobby, SelfPlayer, "Ready", "true");
    }

    public async void PlayerUnready(){
        await UpdateSelfPlayerDataAsync(CurrentLobby, SelfPlayer, "Ready", "false");
    }

    public async void LobbyReady(Lobby lobby){
        await UpdateLobbyDataAsync(lobby, "Ready", "true");
    }


    private async UniTask UpdateLobbyDataAsync(Lobby lobby, string key, string value, bool isPrivate = false)
    {

        Dictionary<string, DataObject> dataCurr = lobby.Data ?? new Dictionary<string, DataObject>();
        dataCurr[key] = new DataObject(isPrivate ? DataObject.VisibilityOptions.Private : DataObject.VisibilityOptions.Public, value);

        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr };
        await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, updateOptions);
    }

    public async UniTask UpdateSelfPlayerDataAsync(Lobby lobby, Player player, string key, string value, bool isPrivate = false)
    {
        Dictionary<string, PlayerDataObject> dataCurr = player.Data ?? new Dictionary<string, PlayerDataObject>();
        dataCurr[key] = new PlayerDataObject(isPrivate ? PlayerDataObject.VisibilityOptions.Private : PlayerDataObject.VisibilityOptions.Public, value);

        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions { Data = dataCurr };
        await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, player.Id, updateOptions);
    }

}
