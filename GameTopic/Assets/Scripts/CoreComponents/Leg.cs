using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : BaseCoreComponent , IBodyControlable {
    public BaseCoreComponent body { get; private set; }

    HashSet<GameObject> touchObj = new HashSet<GameObject>();

    protected override void Awake() {
        body = this;


        base.Awake();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var obj = collision.gameObject.GetComponent<BaseCoreComponent>();
        if (obj == null) {
            touchObj.Add(collision.gameObject);
            body.Root.BodyRigidbody.gravityScale = 0;
        } else {
            if (!obj.HasTheSameRootWith(body)) {
                touchObj.Add(collision.gameObject);
                body.Root.BodyRigidbody.gravityScale = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        touchObj.Remove(collision.gameObject);
        if (touchObj.Count == 0) {
            body.Root.BodyRigidbody.gravityScale = 1;
        }
    }
}
