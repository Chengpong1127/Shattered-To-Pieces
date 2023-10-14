using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomInfo : MonoBehaviour
{
    public Button btn;
    public TMP_Text tmp_t;

    public Action<string> OnClickRoom = null;

    private void Awake() {
        btn.onClick.AddListener(() => {OnClickRoom?.Invoke(tmp_t.text); });
    }
}
