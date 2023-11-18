using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneProccess : MonoBehaviour
{
    // host
    // show room list
    // lobby

    [SerializeField] Button HostBtn;
    [SerializeField] TMP_InputField HostNameInputField;

    [SerializeField] Button RoomListBtn;
    [SerializeField] GameObject AddRoomBtnPrefab;
    [SerializeField] GameObject RoomListLayoutParent;
    [SerializeField] Animator RoomListAnimator;
    List<RoomInfo> RoomBtns = new List<RoomInfo>();

    [SerializeField] GameObject LobbyPlayerInfoPrefab;
    [SerializeField] GameObject LobbyLayoutParent;
    List<LobbyPlayerInfo> infos = new List<LobbyPlayerInfo>();
    bool IsPlayerReady = false;
    bool rgeistListener = false;

    private void Awake() {
        HostBtn.onClick.AddListener(SubmitHostName);

        RoomListBtn.onClick.AddListener(UpdateConnectRoom);
        RoomListBtn.onClick.AddListener(RegistListener);
        RoomListLayoutParent.SetActive(false);
    }

    private void OnDestroy() {
        if (!rgeistListener) { return; }
        LocalGameManager.Instance.LobbyManager.OnPlayerJoinOrLeave -= RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerReady -= UpdatePlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerUnready -= UpdatePlayerInfo;
    }


    #region RoomList
    async void UpdateConnectRoom() {
        // get room name list by GM.

        var lgm = LocalGameManager.Instance;
        var lobbyLst = await lgm.GetAllAvailableLobby();
        List<string> roomNames = new List<string>();

        for (int i = 0; i < lobbyLst.Length; ++i) {
            roomNames.Add(lobbyLst[i].Name);
        }

        RoomBtns.ForEach(btn => {
            Destroy(btn.gameObject);
        });
        RoomBtns.Clear();
        roomNames.ForEach(roomName => {
            var igo = Instantiate(AddRoomBtnPrefab, RoomListLayoutParent.transform, false);
            var rif = igo.GetComponent<RoomInfo>();
            RoomBtns.Add(rif);
        });
        for (int i = 0; i < roomNames.Count; ++i) {
            RoomBtns[i].gameObject.SetActive(true);
            RoomBtns[i].tmp_t.text = roomNames[i];
            RoomBtns[i].OnClickRoom = ClickRoomBtn;
        }

        // play animation & show room list
        StartCoroutine(ShowList());

    }

    IEnumerator ShowList() {
        RoomListAnimator.SetTrigger("Play");
        yield return new WaitWhile(() => RoomListAnimator.GetCurrentAnimatorStateInfo(0).IsName("Playing"));
        yield return new WaitWhile(() => !RoomListAnimator.GetCurrentAnimatorStateInfo(0).IsName("Playing"));

        RoomListLayoutParent.SetActive(true);
        yield return null;
    }

    async void ClickRoomBtn(string roomName) {
        // submit enter room
        Debug.Log("Choose room : " + roomName);

        var lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        var lobby = lobbies.Where(lb => { return lb.Name == roomName; }).FirstOrDefault();
        if (lobby == null) { Debug.Log("Choose room is null"); return; }
        LocalGameManager.Instance.JoinLobby(lobby).Forget();
    }

    #endregion



    #region HostBtn
    void SubmitHostName() {
        var hn = HostNameInputField.text;
        RegistListener();
        //LocalGameManager.Instance.CreateLobby(hn).Forget();
    }
    #endregion


    #region LobbyInfo

    void RegistPlayerInfo() {
        Debug.Log("call RegistPlayerInfo");

        infos.ForEach(info => {
            if (info.IsDestroyed()) { return; }
            Destroy(info.gameObject);
        });
        infos.Clear();

        LocalGameManager.Instance.LobbyManager.CurrentLobby.Players.ForEach(player => {
            var pinfo = Instantiate(LobbyPlayerInfoPrefab, LobbyLayoutParent.transform, false).GetComponent<LobbyPlayerInfo>();
            infos.Add(pinfo);
            if (player.Id != LocalGameManager.Instance.LobbyManager.SelfPlayer.Id) {
                pinfo.readyBtn.gameObject.SetActive(false);
            } else {
                pinfo.readyBtn.onClick.AddListener(ReadySwitch);
            }

            pinfo.tmp_t.text = player.Data["Ready"].Value == "true" ? "Ready" : "not ready";
        });

        Debug.Log("player count : " + infos.Count);
    }

    void UpdatePlayerInfo(Player player) {
        Debug.Log("call UpdatePlayerInfo");
        int index = 0;

        LocalGameManager.Instance.LobbyManager.CurrentLobby.Players.ForEach(player => {
            if (infos.Count <= index) { return; }
            infos[index].tmp_t.text = player.Data["Ready"].Value == "true" ? "Ready" : "not ready";
            index++;
        });
    }

    void ReadySwitch() {
        IsPlayerReady = IsPlayerReady ? false : true;
        if (IsPlayerReady) { LocalGameManager.Instance.PlayerReady(); } else { LocalGameManager.Instance.PlayerUnready(); }
    }

    #endregion



    void RegistListener() {
        rgeistListener = true;
        LocalGameManager.Instance.LobbyManager.OnPlayerJoinOrLeave -= RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerReady -= UpdatePlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerUnready -= UpdatePlayerInfo;

        LocalGameManager.Instance.LobbyManager.OnPlayerJoinOrLeave += RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerReady += UpdatePlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerUnready += UpdatePlayerInfo;
    }
}