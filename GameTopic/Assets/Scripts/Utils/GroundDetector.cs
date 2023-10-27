using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class GroundDetector: MonoBehaviour{
    public bool IsGrounded { get; private set; }
    private void OnTriggerStay2D(Collider2D other) {
        if (other.GetComponentInParent<Taggable>()?.HasTag("Ground") ?? false) {
            IsGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        IsGrounded = false;
    }
}