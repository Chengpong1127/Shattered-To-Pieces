using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UI;
using static LobbyManager;

public class LobbyInteract : MonoBehaviour
{
    [SerializeField] GameObject LobbyPlayerInfoPrefab;
    List<LobbyPlayerInfo> infos = new List<LobbyPlayerInfo>();

    bool IsPlayerReady = false;

    private void Start() {
        // LocalGameManager.Instance.LobbyManager is null

        // LocalGameManager.Instance.LobbyManager.OnPlayerJoinOrLeave += RegistPlayerInfo;
        // LocalGameManager.Instance.LobbyManager.OnPlayerReady += UpdatePlayerInfo;
        // LocalGameManager.Instance.LobbyManager.OnPlayerUnready += UpdatePlayerInfo;
    }
    void RegistPlayerInfo() {
        Debug.Log("server call RegistPlayerInfo");
        RegistPlayerInfo_ClientRpc();
    }
    [ClientRpc]
    void RegistPlayerInfo_ClientRpc() {
        Debug.Log("client call RegistPlayerInfo rpc");

        infos.ForEach(info => {
            Destroy(info.gameObject);
        });
        infos.Clear();

        LocalGameManager.Instance.LobbyManager.CurrentLobby.Players.ForEach(player => {
            var pinfo = Instantiate(LobbyPlayerInfoPrefab, this.gameObject.transform, false).GetComponent<LobbyPlayerInfo>();
            infos.Add(pinfo);
            if (player != LocalGameManager.Instance.LobbyManager.SelfPlayer) {
                pinfo.readyBtn.gameObject.SetActive(false);
            } else {
                pinfo.readyBtn.onClick.AddListener(ReadySwitch);
            }

            pinfo.tmp_t.text = player.Data["Ready"].Value == "true" ? "Ready" : "";
        });
    }

    void UpdatePlayerInfo(Player player) {
        Debug.Log("server call UpdatePlayerInfo");
        UpdatePlayerInfo_ClientRpc();
    }

    [ClientRpc]
    void UpdatePlayerInfo_ClientRpc() {
        Debug.Log("client call UpdatePlayerInfo rpc");
        int index = 0;

        LocalGameManager.Instance.LobbyManager.CurrentLobby.Players.ForEach(player => {
            if(infos.Count <= index) { return; }
            infos[index].tmp_t.text = player.Data["Ready"].Value == "true" ? "Ready" : "";
            index++;
        });
    }

    void ReadySwitch() {
        IsPlayerReady = IsPlayerReady ? false : true;
        if (IsPlayerReady) { LocalGameManager.Instance.PlayerReady(); }
        else { LocalGameManager.Instance.PlayerUnready(); }
    }
}
