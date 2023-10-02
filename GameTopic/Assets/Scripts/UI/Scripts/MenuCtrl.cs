using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCtrl : MonoBehaviour
{
    private void Awake() {
        gameObject.SetActive(false);
    }
    public void SwitchActive() {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}