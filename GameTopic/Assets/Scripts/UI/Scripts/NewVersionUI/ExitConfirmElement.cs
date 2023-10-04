using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitConfirmElement : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
    [SerializeField] ExitCtrl exitCtrl;
    [SerializeField] bool IsExit;

    RectTransform selfTransform;
    private void Start() {
        selfTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        exitCtrl.OnHever.Invoke(selfTransform.position.y);
    }

    public void OnPointerClick(PointerEventData eventData) {
        exitCtrl.OnClick.Invoke(IsExit);
    }
}
