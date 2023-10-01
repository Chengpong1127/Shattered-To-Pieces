using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceCtrl : MonoBehaviour
{
    [SerializeField] TMP_Text selfTMP;

    public void SetPrice(int p) {
        if(selfTMP == null || p < 0) { return; }

        selfTMP.text = p.ToString();
    }
}
