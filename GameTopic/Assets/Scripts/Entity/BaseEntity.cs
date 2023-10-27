using UnityEngine;
using Unity.Netcode;
using System;

public abstract class BaseEntity : NetworkBehaviour
{

    public virtual Transform BodyTransform => transform;
    public virtual Rigidbody2D BodyRigidbody => bodyRigidbody;
    public virtual Collider2D[] BodyColliders => bodyColliders;
    public virtual Animator BodyAnimator => bodyAnimator;
    public virtual Renderer[] BodyRenderers => bodyRenderers;
    public virtual Taggable Taggable { get; private set; }


    public event Action OnEntityDied;

    [Header("BaseEntity Setting")]
    [SerializeField]
    private Rigidbody2D bodyRigidbody;
    [SerializeField]
    private Collider2D[] bodyColliders;
    [SerializeField]
    private Animator bodyAnimator;
    [SerializeField]
    private Renderer[] bodyRenderers;
    protected virtual void Awake()
    {
        if (BodyTransform == null) Debug.LogWarning($"The BaseEntity: {gameObject.name} doesn't set BodyTransform.");
        if (BodyRigidbody == null) Debug.LogWarning($"The BaseEntity: {gameObject.name} doesn't set BodyRigidbody.");
        if (BodyColliders == null) Debug.LogWarning($"The BaseEntity: {gameObject.name} doesn't set BodyColliders.");
        if (BodyRenderers == null) Debug.LogWarning($"The BaseEntity: {gameObject.name} doesn't set BodyRenderers.");
        Taggable = GetComponent<Taggable>() ?? gameObject.AddComponent<Taggable>();
        if (Taggable == null) Debug.LogWarning($"The BaseEntity: {gameObject.name} doesn't set Taggable.");
    }

    protected virtual void Start()
    {
        
    }

    public virtual void Repel(Vector2 force){
        if (BodyRigidbody != null){
            BodyRigidbody.AddForce(force, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// This method will immediately destroy the BodyTransform of an entity.
    /// </summary>
    public virtual void Die(){
        OnEntityDied?.Invoke();
        GameEvents.GameComponentEvents.OnEntityDied?.Invoke(this);
        NetworkObject?.Despawn(true);
    }
}