using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameUI : MonoBehaviour {
    [SerializeField] public TMP_Text resultTMP;
    private void Awake() {
        gameObject.SetActive(false);
    }
}
