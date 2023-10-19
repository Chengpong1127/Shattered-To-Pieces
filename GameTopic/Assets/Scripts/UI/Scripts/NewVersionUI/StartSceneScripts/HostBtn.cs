using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HostBtn : MonoBehaviour
{
    [SerializeField] Button hostBtn;
    [SerializeField] TMP_InputField HostNameInputField;
    // [SerializeField] object LobbyInfoObj;

    private void Awake() {
        hostBtn.onClick.AddListener(SubmitHostName);
    }


    void SubmitHostName() {
        var hn = HostNameInputField.text;
        LocalGameManager.Instance.CreateLobby(hn);
        
        // var lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        // var lobby = lobbies.Where(lb => { return lb.Name == hn; }).FirstOrDefault();
        // if(lobby == null) { return; }
        // LocalGameManager.Instance.JoinLobby(lobby);
        // update LobbyInfoObj
    }
}
