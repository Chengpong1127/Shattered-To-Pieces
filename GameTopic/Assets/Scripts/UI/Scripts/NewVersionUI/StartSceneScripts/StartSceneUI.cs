using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;



public class StartSceneUI : MonoBehaviour
{
    [SerializeField] Animator UIanimator;
    
    [SerializeField] UniBtn AssemblyRoomBtn;
    [SerializeField] UniBtn HostBtn;
    [SerializeField] UniBtn ListRoomBtn;
    [SerializeField] UniBtn Back;

    [SerializeField] GameObject LayoutObj;
    [SerializeField] TMP_InputField RoomNameInput;

    [SerializeField] GameObject RoomBtnPrefab;
    List<RoomInfo> RoomBtns = new List<RoomInfo>();
    [SerializeField] GameObject LobbyPlayerInfoPrefab;
    List<LobbyPlayerInfo> infos = new List<LobbyPlayerInfo>();
    bool IsPlayerReady = false;



    private void Awake() {

        AssemblyRoomBtn.EnterHover += () => { UIanimator.SetBool("MainPageHoving", true); };
        AssemblyRoomBtn.ExitHover += () => { UIanimator.SetBool("MainPageHoving", false); };
        HostBtn.EnterHover += () => { UIanimator.SetBool("MainPageHoving", true); };
        HostBtn.ExitHover += () => { UIanimator.SetBool("MainPageHoving", false); };
        ListRoomBtn.EnterHover += () => { UIanimator.SetBool("MainPageHoving", true); };
        ListRoomBtn.ExitHover += () => { UIanimator.SetBool("MainPageHoving", false); };

        ListRoomBtn.OnClick += () => { UpdateConnectRoom(); UIanimator.SetTrigger("ListRoom"); };
        HostBtn.OnClick += () => { UIanimator.SetTrigger("CreatRoom"); };
        Back.OnClick += () => { UIanimator.SetTrigger("Back"); };
    }


    void Start() {
        StartCoroutine(RegistEvent());
    }

    private void OnDestroy() {
        LocalGameManager.Instance.LobbyManager.OnPlayerJoinOrLeave -= RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerReady -= RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerUnready -= RegistPlayerInfo;
    }

    IEnumerator RegistEvent() {
        yield return new WaitUntil(() => LocalGameManager.Instance != null);
        LocalGameManager.Instance.LobbyManager.OnPlayerJoinOrLeave += RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerReady += RegistPlayerInfo;
        LocalGameManager.Instance.LobbyManager.OnPlayerUnready += RegistPlayerInfo;
        yield return null;
    }


    async void UpdateConnectRoom() {
        var lgm = LocalGameManager.Instance;
        var lobbyLst = await lgm.GetAllAvailableLobby();
        List<string> roomNames = new List<string>();

        for (int i = 0; i < lobbyLst.Length; ++i) {
            roomNames.Add(lobbyLst[i].Name);
        }

        infos.ForEach(info => {
            if (info.IsDestroyed()) { return; }
            Destroy(info.gameObject);
        });
        infos.Clear();
        RoomBtns.ForEach(btn => {
            Destroy(btn.gameObject);
        });
        RoomBtns.Clear();
        roomNames.ForEach(roomName => {
            var igo = Instantiate(RoomBtnPrefab, LayoutObj.transform, false);
            var rif = igo.GetComponent<RoomInfo>();
            RoomBtns.Add(rif);
        });
        for (int i = 0; i < roomNames.Count; ++i) {
            RoomBtns[i].gameObject.SetActive(true);
            RoomBtns[i].tmp_t.text = roomNames[i];
            RoomBtns[i].OnClickRoom = ClickRoomBtn;
        }
    }
    async void ClickRoomBtn(string roomName) {
        var lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        var lobby = lobbies.Where(lb => { return lb.Name == roomName; }).FirstOrDefault();
        if (lobby == null) { Debug.Log("Choose room is null"); UIanimator.SetTrigger("Back"); return; }
        LocalGameManager.Instance.JoinLobby(lobby);
    }
    void RegistPlayerInfo(Player player) {
        RegistPlayerInfo();
    }
    void RegistPlayerInfo() {
        infos.ForEach(info => {
            if (info.IsDestroyed()) { return; }
            Destroy(info.gameObject);
        });
        infos.Clear();
        RoomBtns.ForEach(btn => {
            Destroy(btn.gameObject);
        });
        RoomBtns.Clear();

        LocalGameManager.Instance.LobbyManager.CurrentLobby.Players.ForEach(player => {
            var pinfo = Instantiate(LobbyPlayerInfoPrefab, LayoutObj.transform, false).GetComponent<LobbyPlayerInfo>();
            infos.Add(pinfo);
            if (player.Id != LocalGameManager.Instance.LobbyManager.SelfPlayer.Id) {
                pinfo.readyBtn.gameObject.SetActive(false);
            } else {
                pinfo.readyBtn.onClick.AddListener(ReadySwitch);
            }

            pinfo.tmp_t.text = player.Data["Ready"].Value == "true" ? "Ready" : "not ready";
        });
    }
    void ReadySwitch() {
        IsPlayerReady = IsPlayerReady ? false : true;
        if (IsPlayerReady) { LocalGameManager.Instance.PlayerReady(); } else { LocalGameManager.Instance.PlayerUnready(); }
    }


    
}
