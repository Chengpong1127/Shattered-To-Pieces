using UnityEngine;
using Unity.Netcode;

public abstract class BaseEntity : NetworkBehaviour
{

    public virtual Transform BodyTransform { get; protected set; }
    public virtual Rigidbody2D BodyRigidbody { get; protected set; }
    public virtual Collider2D[] BodyColliders { get; protected set; }
    public virtual Animator BodyAnimator { get; protected set; }

    [Header("BaseEntity Setting")]
    [SerializeField]
    private Transform bodyTransform;
    [SerializeField]
    private Rigidbody2D bodyRigidbody;
    [SerializeField]
    private Collider2D[] bodyColliders;
    [SerializeField]
    private Animator bodyAnimator;
    protected virtual void Awake()
    {
        BodyTransform = bodyTransform;
        BodyRigidbody = bodyRigidbody;
        BodyColliders = bodyColliders;
        BodyAnimator = bodyAnimator;

        if (BodyTransform == null) Debug.LogWarning("BodyTransform is not set.");
        if (BodyRigidbody == null) Debug.LogWarning("BodyRigidbody is not set.");
        if (BodyColliders == null) Debug.LogWarning("BodyColliders is not set.");
        if (BodyAnimator == null) Debug.LogWarning("BodyAnimator is not set.");
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
        NetworkObject?.Despawn(true);
    }
}