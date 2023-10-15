using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Cysharp.Threading.Tasks;

public class LobbyManager: Singleton<LobbyManager>
{
    public Lobby CurrentLobby;
    public async UniTask CreateLobby(string lobbyName, int maxPlayers, Player player, CreateLobbyOptions options = null){
        var createLobbyOptions = options ?? new CreateLobbyOptions(){
            IsPrivate = false,
            IsLocked = false,
            Player = player};
        CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
        BindLobbyEvents(CurrentLobby.Id);
        Debug.Log("Lobby Created with Lobby ID: " + CurrentLobby.Id);
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

    public async UniTask<bool> JoinLobby(Lobby lobby, Player player){
        if (CurrentLobby != null)
        {
            Debug.Log("Already Joined Lobby");
            return false;
        }
        JoinLobbyByIdOptions quickJoinLobbyOptions = new JoinLobbyByIdOptions { Player = player };
        CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, quickJoinLobbyOptions);
        BindLobbyEvents(CurrentLobby.Id);
        Debug.Log("Joined new lobby with ID: " + CurrentLobby.Id);
        return true;
    }

    private void BindLobbyEvents(string lobbyID){
        LobbyEventCallbacks LobbyEventCallbacks = new LobbyEventCallbacks();
        LobbyEventCallbacks.LobbyChanged += (changes) => {
            Debug.Log("Lobby Changed");
        };
        LobbyEventCallbacks.PlayerJoined += (players) => {
            Debug.Log("Player Joined");
        };
        LobbyEventCallbacks.PlayerLeft += (players) => {
            Debug.Log("Player Left");
        };
        LobbyEventCallbacks.DataChanged += (data) => {
            Debug.Log("Data Changed");
            GameEvents.LobbyEvents.OnLobbyDataChanged.Invoke(ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.DataRemoved += (data) => {
            Debug.Log("Data Removed");
            GameEvents.LobbyEvents.OnLobbyDataChanged.Invoke(ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.DataAdded += (data) => {
            Debug.Log("Data Added");
            GameEvents.LobbyEvents.OnLobbyDataChanged.Invoke(ConvertLobbyData(CurrentLobby.Data));
        };
        LobbyEventCallbacks.PlayerDataChanged += (data) => {
            Debug.Log("Player Data Changed");
        };
        LobbyEventCallbacks.PlayerDataRemoved += (data) => {
            Debug.Log("Player Data Removed");
        };
        LobbyEventCallbacks.PlayerDataAdded += (data) => {
            Debug.Log("Player Data Added");
        };
        LobbyEventCallbacks.LobbyDeleted += () => {
            Debug.Log("Lobby Deleted");
        };
        LobbyEventCallbacks.KickedFromLobby += () => {
            Debug.Log("Kicked From Lobby");
        };
        LobbyEventCallbacks.LobbyEventConnectionStateChanged += (state) => {
            Debug.Log("Lobby Event Connection State Changed: " + state);
        };
        LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, LobbyEventCallbacks);
    }
    private Dictionary<string, string> ConvertLobbyData(Dictionary<string, DataObject> data){
        Dictionary<string, string> result = new Dictionary<string, string>();
        foreach (var item in data)
        {
            result.Add(item.Key, item.Value.Value);
        }
        return result;
    }


    private async UniTask UpdateLobbyDataAsync(string key, string value, bool isPrivate = false)
    {
        if (CurrentLobby == null)
            return;

        Dictionary<string, DataObject> dataCurr = CurrentLobby.Data ?? new Dictionary<string, DataObject>();
        dataCurr[key] = new DataObject(isPrivate ? DataObject.VisibilityOptions.Private : DataObject.VisibilityOptions.Public, value);

        UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr };
        CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(CurrentLobby.Id, updateOptions);
    }

    private async UniTask UpdateSelfPlayerDataAsync(string key, string value, Player player, bool isPrivate = false)
    {
        if (CurrentLobby == null)
            return;

        Dictionary<string, PlayerDataObject> dataCurr = player.Data ?? new Dictionary<string, PlayerDataObject>();
        dataCurr[key] = new PlayerDataObject(isPrivate ? PlayerDataObject.VisibilityOptions.Private : PlayerDataObject.VisibilityOptions.Public, value);

        UpdatePlayerOptions updateOptions = new UpdatePlayerOptions { Data = dataCurr };
        await LobbyService.Instance.UpdatePlayerAsync(CurrentLobby.Id, player.Id, updateOptions);
    }

}
