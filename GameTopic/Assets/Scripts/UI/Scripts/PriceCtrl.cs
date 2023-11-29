using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceCtrl : MonoBehaviour
{
    [SerializeField] TMP_Text selfTMP;
    [SerializeField] Color normalColor;
    [SerializeField] Color notEnoughColor;
    void Awake()
    {
        Debug.Assert(selfTMP != null);
    }

    public void SetPrice(int p) {
        selfTMP.text = p.ToString();
    }
    public void SetNormalColor() {
        selfTMP.color = normalColor;
    }
    public void SetNotEnoughColor() {
        selfTMP.color = notEnoughColor;
    }
}
