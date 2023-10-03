using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class FakeButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string Hover;
    [SerializeField] string Press;
    [SerializeField] string Release;
    [SerializeField] string Idle;
    [SerializeField] public Animator animator;

    private void Start() {
        Debug.Log(animator?.name);
    }

    public void OnPointerDown(PointerEventData pointerEventData) {
        animator?.SetTrigger(Press);
    }
    public void OnPointerUp(PointerEventData pointerEventData) {
        animator?.SetTrigger(Release);
    }
    public void OnPointerEnter(PointerEventData pointerEventData) {
        animator?.SetTrigger(Hover);
    }
    public void OnPointerExit(PointerEventData pointerEventData) {
        animator?.SetTrigger(Idle);
    }
}
