using System.Collections.Generic;
using UnityEngine;

public class ControlRoom : BaseCoreComponent, ICharacterCtrl {
    public bool Landing { get; private set; }
    bool Pushing;
    int Moving;

    Vector2 replaceVec = Vector2.zero;

    [SerializeField] Collider2D LandCheckCollider;
    static ContactFilter2D filter = new();
    List<Collider2D> collisionResult = new();
    protected override void Awake() {
        base.Awake();

        // filter.useLayerMask = false;
    }

    private void Update() {
        Landing = false;
        if (LandCheckCollider.OverlapCollider(filter, collisionResult) != 0) {
            collisionResult.ForEach(collider => {
                var obj = collider.gameObject.GetComponent<BaseCoreComponent>();
                if (obj == null || !obj.HasTheSameRootWith(this)) { Landing = true; } // Need a Tag to confirm landable Object
            });
        }

        if (Moving <= 0 && !Pushing && Landing) {
            Bondage();
        }
        Moving--;
    }

    public void Move(Vector3 Motion, ForceMode2D Mode) {
        if(Pushing) { return; }
        Moving = 20;
        this.BodyRigidbody.AddForce(Motion, Mode);
    }
    public void Push(Vector3 Motion) {
        Bondage();
        Pushing = true;
        this.BodyRigidbody.AddForce(Motion, ForceMode2D.Impulse);
    }
    public void Bondage() {
        Pushing = false;
        Moving = 0;
        replaceVec.y = this.BodyRigidbody.velocity.y;
        this.BodyRigidbody.velocity = replaceVec;
        this.BodyRigidbody.angularVelocity = 0;
    }
}