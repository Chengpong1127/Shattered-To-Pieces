using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertCtrl : MonoBehaviour
{
    public void SwitchActive() {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
