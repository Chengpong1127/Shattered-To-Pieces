using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UniBtn : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    
    public Action OnClick = null;
    public Action EnterHover = null;
    public Action ExitHover = null;
    
    public void OnPointerClick(PointerEventData eventData) {
        OnClick?.Invoke();
    }
    public void OnPointerEnter(PointerEventData eventData) {
        EnterHover?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData) {
        ExitHover?.Invoke();
    }
}
