using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;

public class ControlRoom : BaseCoreComponent, ICharacterCtrl {
    public bool Landing { get; private set; }
    int Pushing;
    int Moving;

    Vector2 replaceVec = Vector2.zero;

    [SerializeField] Collider2D LandCheckCollider;
    [SerializeField] GameplayEffectScriptableObject LandCheckGE;
    GameplayEffectSpec LandCheckGESpec;
    static ContactFilter2D filter = new();
    List<Collider2D> collisionResult = new();
    protected override void Awake() {
        base.Awake();
        Bondage();
        LandCheckGESpec = this.AbilitySystemCharacter.MakeOutgoingSpec(LandCheckGE);
    }

    private void Update() {
        Landing = false;
        if (LandCheckCollider.OverlapCollider(filter, collisionResult) != 0) {
            collisionResult.ForEach(collider => {
                var obj = collider.gameObject.GetComponent<BaseCoreComponent>();
                
                if(obj != null && obj.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(LandCheckGESpec)) {
                    Landing = true;
                    Pushing--;
                }

                // if (obj == null || !obj.HasTheSameRootWith(this)) {
                //     Landing = true;
                //     Pushing--;
                // } // Need a Tag to confirm landable Object
            });
        }

        if (Moving <= 0 && Pushing <= 0 && Landing) {
            Bondage();
        }
        Moving--;
    }

    public void Move(Vector3 Motion, ForceMode2D Mode) {
        if(Pushing > 0) { return; }
        Moving = 20;
        // this.BodyRigidbody.AddForce(Motion, Mode);
        replaceVec = Motion;
        replaceVec.y += BodyRigidbody.velocity.y;
        BodyRigidbody.velocity = replaceVec;
    }
    public void Push(Vector3 Motion) {
        Bondage();
        Pushing = 20;
        this.BodyRigidbody.AddForce(Motion, ForceMode2D.Impulse);
    }
    public void Bondage() {
        Pushing = 0;
        Moving = 0;
        replaceVec.y = this.BodyRigidbody.velocity.y;
        replaceVec.x = 0;
        this.BodyRigidbody.velocity = replaceVec;
        this.BodyRigidbody.angularVelocity = 0;
    }
}