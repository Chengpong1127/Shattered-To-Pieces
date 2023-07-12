using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCtrl : MonoBehaviour
{
    public void SwitchActive() {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
