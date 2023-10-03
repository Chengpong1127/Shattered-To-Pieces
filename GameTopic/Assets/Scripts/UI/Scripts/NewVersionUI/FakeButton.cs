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
        Debug.Log("Down " + gameObject.name);
        animator?.SetTrigger(Press);
    }
    public void OnPointerUp(PointerEventData pointerEventData) {
        Debug.Log("Up " + gameObject.name);
        animator?.SetTrigger(Release);
    }
    public void OnPointerEnter(PointerEventData pointerEventData) {
        Debug.Log("Enter " + gameObject.name);
        animator?.SetTrigger(Hover);
    }
    public void OnPointerExit(PointerEventData pointerEventData) {
        Debug.Log("Exit " + gameObject.name);
        animator?.SetTrigger(Idle);
    }
}
