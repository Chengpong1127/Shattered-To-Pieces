using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    void UpdateConnectRoom() {
        // get room name list by GM.
        List<string> roomNames = new List<string>();

        if (RoomBtns.Count < roomNames.Count) {
            var igo = Instantiate(AddRoomBtnPrefab, LayoutParent.transform, false);
            var rif = igo.GetComponent<RoomInfo>();
            RoomBtns.Add(rif);
        } else {
            for(int i= roomNames.Count;i < RoomBtns.Count; ++i) {
                RoomBtns[i].gameObject.SetActive(false);
            }
        }

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

    void ClickRoomBtn(string roomName) {
        // submit enter room
        Debug.Log("Choose room : " + roomName);
    }
}
