using UnityEngine;
using Unity.Netcode;

public abstract class BaseEntity : NetworkBehaviour
{

    public virtual Transform BodyTransform => transform;
    public virtual Rigidbody2D BodyRigidbody => bodyRigidbody;
    public virtual Collider2D[] BodyColliders => bodyColliders;
    public virtual Animator BodyAnimator => bodyAnimator;
    public virtual Renderer[] BodyRenderers => bodyRenderers;

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
        GameEvents.GameComponentEvents.OnEntityDied?.Invoke(this);
        NetworkObject?.Despawn(true);
    }
    [ClientRpc]
    public void BodyRigibodyAddForce_ClientRpc(Vector2 force, ForceMode2D mode){
        if (IsOwner){
            BodyRigidbody.AddForce(force, mode);
        }
    }
}