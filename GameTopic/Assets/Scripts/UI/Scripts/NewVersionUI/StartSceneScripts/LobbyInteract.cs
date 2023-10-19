using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInteract : MonoBehaviour
{
    [SerializeField] TMP_Text readyText;
    [SerializeField] Button readyBtn;

    bool IsPlayerReady = false;

    private void Awake() {
        readyBtn.onClick.AddListener(ReadySwitch);
    }

    void ReadySwitch() {
        IsPlayerReady = IsPlayerReady ? false : true;
        if (IsPlayerReady) { LocalGameManager.Instance.PlayerReady(); readyText.text = "Player is ready."; }
        else { LocalGameManager.Instance.PlayerUnready(); readyText.text = "You're not ready."; }
    }
}
