using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Unity.Netcode;

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
                var obj = collider.gameObject.GetComponent<Entity>();
                
                if(obj != null && obj.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(LandCheckGESpec)) {
                    Landing = true;
                    Pushing--;
                }
            });
        }

        if (Moving <= 0 && Pushing <= 0 && Landing) {
            Bondage();
            Moving = 0;
            Pushing = 0;
        }
        Moving--;
    }

    public void Move(Vector2 Motion) {
        if(Pushing > 0) { return; }
        Moving = 20;
        BodyRigidbody.velocity = Motion;
    }

    public void VerticalMove(float Motion) {
        if (Pushing > 0) { return; }
        Moving = 20;
        replaceVec.x = BodyRigidbody.velocity.x;
        replaceVec.y = Motion;
        BodyRigidbody.velocity = replaceVec;
    }
    public void HorizontalMove(float Motion) {
        if (Pushing > 0) { return; }
        Moving = 20;
        replaceVec.x = Motion;
        replaceVec.y = BodyRigidbody.velocity.y;
        BodyRigidbody.velocity = replaceVec;
    }
    public void AddForce(Vector2 Motion, ForceMode2D Mode) {
        Moving = 20;
        BodyRigidbody.AddForce(Motion, Mode);
    }

    public void Push(Vector2 Motion) {
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


    public void ToggleAssembly(ulong playerID){
        var parameters = new ClientRpcParams{
            Send = new ClientRpcSendParams{
                TargetClientIds = new List<ulong>{playerID}
            }
        };
        ToggleAssembly_ClientRpc(parameters);
    }

    [ClientRpc]
    private void ToggleAssembly_ClientRpc(ClientRpcParams clientRpcParams = default){
        var device = Utils.GetLocalPlayerDevice();
        device.AssemblyController.enabled = !device.AssemblyController.enabled;
    }
}