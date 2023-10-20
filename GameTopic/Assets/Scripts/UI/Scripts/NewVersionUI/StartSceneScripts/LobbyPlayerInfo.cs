using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerInfo : MonoBehaviour
{
    [SerializeField] public TMP_Text tmp_t;
    [SerializeField] public Button readyBtn;

    private void Awake() {
        tmp_t.text = string.Empty;
    }

}
