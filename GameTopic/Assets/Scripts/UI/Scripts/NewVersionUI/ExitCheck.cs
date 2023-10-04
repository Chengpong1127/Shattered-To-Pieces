using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitCheck : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject ExitMenu;
    public void OnPointerClick(PointerEventData eventData) {
        ExitMenu?.SetActive(true);
    }
}