using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ClientBtn : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject AddRoomBtnPrefab;
    [SerializeField] GameObject LayoutParent;
    [SerializeField] Animator animator;
    List<RoomInfo> RoomBtns = new List<RoomInfo>();


    private void Awake() {
        button.onClick.AddListener(UpdateConnectRoom);
        LayoutParent.SetActive(false);
    }

    async void UpdateConnectRoom() {
        // get room name list by GM.

        var lgm = LocalGameManager.Instance;
        var lobbyLst = await lgm.GetAllAvailableLobby();
        List<string> roomNames = new List<string>();

        for(int i=0;i< lobbyLst.Length; ++i) {
            roomNames.Add(lobbyLst[i].Name);
        }

        RoomBtns.ForEach(btn => {
            Destroy(btn.gameObject);
        });
        RoomBtns.Clear();
        roomNames.ForEach(roomName => {
            var igo = Instantiate(AddRoomBtnPrefab, LayoutParent.transform, false);
            var rif = igo.GetComponent<RoomInfo>();
            RoomBtns.Add(rif);
        });
        for(int i = 0; i < roomNames.Count; ++i) {
            RoomBtns[i].gameObject.SetActive(true);
            RoomBtns[i].tmp_t.text = roomNames[i];
            RoomBtns[i].OnClickRoom = ClickRoomBtn;
        }

        // play animation & show room list
        StartCoroutine(ShowList());
        
    }

    IEnumerator ShowList() {
        animator.SetTrigger("Play");
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Playing"));
        yield return new WaitWhile(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Playing"));

        LayoutParent.SetActive(true);
        yield return null;
    }

    async void ClickRoomBtn(string roomName) {
        // submit enter room
        Debug.Log("Choose room : " + roomName);

        var lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        var lobby = lobbies.Where(lb => { return lb.Name == roomName; }).FirstOrDefault();
        if(lobby == null) { return; }
        LocalGameManager.Instance.JoinLobby(lobby);
    }
}
