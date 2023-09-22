using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Unity.Netcode;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTagNamespace.Authoring;

public class ControlRoom : BaseCoreComponent, ICharacterCtrl {
    public bool Landing { get; private set; }
    int Pushing;
    int Moving;
    Vector2 replaceVec = Vector2.zero;
    [SerializeField] public Material m_Default;
    [SerializeField] public Material m_Invisible;
    [SerializeField] Collider2D LandCheckCollider;
    [SerializeField] GameplayEffectScriptableObject LandCheckGE;
    [SerializeField] GameplayTag LandCheckTag;
    [SerializeField] GameplayEffectScriptableObject VelocityInit;
    [SerializeField] AttributeScriptableObject MovingVelocity;
    GameplayEffectSpec LandCheckGESpec;
    static ContactFilter2D filter = new();
    List<Collider2D> collisionResult = new();
    protected override void Awake() {
        base.Awake();
        Bondage();
        //LandCheckGESpec = this.AbilitySystemCharacter.MakeOutgoingSpec(LandCheckGE);
        //this.AttributeSystemComponent.AddAttributes(MovingVelocity);
        //this.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(this.AbilitySystemCharacter.MakeOutgoingSpec(VelocityInit));
    }

    private void Update() {
        // Landing = false;
        // if (LandCheckCollider.OverlapCollider(filter, collisionResult) != 0) {
        //     collisionResult.ForEach(collider => {
        //         // var obj = collider.gameObject.GetComponent<Entity>();
        //         var obj = collider.gameObject.GetComponent<Taggable>();
                
        //         // if(obj != null && obj.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(LandCheckGESpec)) {
        //         if(obj != null && obj.HasTag(LandCheckTag)) {
        //             Debug.Log(1);
        //             Landing = true;
        //             Pushing--;
        //         }
        //     });
        // }

        // if (Moving <= 0 && Pushing <= 0 && Landing) {
        //     Bondage();
        //     Moving = 0;
        //     Pushing = 0;
        // }
        // Moving--;
    }

    public void Move(Vector2 Motion) {
        if(Pushing > 0) { return; }
        Moving = 2;
        BodyRigidbody.velocity = Motion;
    }

    public void VerticalMove(float Motion) {
        if (Pushing > 0) { return; }
        Moving = 2;
        replaceVec.x = BodyRigidbody.velocity.x;
        replaceVec.y = Motion;
        BodyRigidbody.velocity = replaceVec;
    }
    public void HorizontalMove(float Motion) {
        if (Pushing > 0) { return; }
        Moving = 2;
        replaceVec.x = Motion;
        replaceVec.y = BodyRigidbody.velocity.y;
        BodyRigidbody.velocity = replaceVec;
    }
    public void AddForce(Vector2 Motion, ForceMode2D Mode) {
        Moving = 2;
        BodyRigidbody.AddForce(Motion, Mode);
    }

    public void Push(Vector2 Motion) {
        Bondage();
        Pushing = 2;
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
        var player = Utils.ServerGetBasePlayer(playerID) as GamePlayer;
        player.ToggleAssemblyClientRpc();
    }

    [ClientRpc]
    public void Invisible_ClientRpc(bool isInvisible)
    {
        var player = Utils.GetLocalPlayerDevice();

        //Debug.Assert(player);
        //Debug.Assert(player.SelfDevice != null);
        //Debug.Assert(player.SelfDevice.RootGameComponent != null);
        //Debug.Assert(player.SelfDevice.RootGameComponent.CoreComponent != null);


        //if (player.SelfDevice.RootGameComponent.CoreComponent.GetComponent<ControlRoom>() == this) { return; }

        Invisible(isInvisible);
    }

    public void Invisible(bool isInvisible)
    {
        var baseCoreComponents = this.GetAllChildren();

        foreach (var baseComponent in baseCoreComponents)
        {
            SpriteRenderer s = baseComponent?.GetComponent<SpriteRenderer>();
            if (s != null)
                s.material = isInvisible ? m_Invisible : m_Default;
            else
            {
                for (var i = 1; i < baseComponent.transform.parent.childCount; i++)
                {
                    var anotherChild = baseComponent.transform.parent.GetChild(i);
                    for (var j = 0; j < anotherChild.childCount; j++)
                    {
                        var anotherChild_Renderer = anotherChild.GetChild(j);
                        SpriteRenderer cs = anotherChild_Renderer.gameObject?.GetComponent<SpriteRenderer>();
                        if (cs != null)
                            cs.material = isInvisible ? m_Invisible : m_Default;
                    }
                }
            }
        }
    }
}