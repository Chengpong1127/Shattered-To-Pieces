using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Linq;

public class PlayerListController : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerItemPrefab;
    private List<PlayerItemController> playerItemControllers = new List<PlayerItemController>();


    void Awake()
    {
        Debug.Assert(_playerItemPrefab != null);
    }

    public void SetPlayerList(List<Player> players, List<Player> readyPlayers, int localPlayerIndex)
    {
        playerItemControllers.ForEach(playerItem => Destroy(playerItem.gameObject));
        playerItemControllers = players.Select(player =>
        {
            var playerItem = Instantiate(_playerItemPrefab,transform).GetComponent<PlayerItemController>();
            playerItem.transform.Translate(Vector2.down * 50f);
            PlayerProfile playerProfile = PlayerProfile.FromJson(player.Data["PlayerProfileJson"].Value);
            playerItem.SetPlayer(playerProfile.Name, readyPlayers.Contains(player));
            return playerItem;
        }).ToList();
        playerItemControllers[localPlayerIndex].SetLocalPlayer();
    }
}