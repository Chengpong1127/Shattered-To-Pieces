using UnityEngine;
using Unity.Netcode;

public abstract class BaseEntity : NetworkBehaviour
{
    public virtual int UnitID { get; set; }

    public virtual Transform BodyTransform { get; protected set; }
    public virtual Rigidbody2D BodyRigidbody { get; protected set; }
    public virtual Collider2D BodyCollider { get; protected set; }
    protected virtual void Awake()
    {
        if (!IsServer) return;
        BodyTransform = transform;
        BodyRigidbody = GetComponent<Rigidbody2D>();
        BodyCollider = GetComponent<Collider2D>();
        if (NetworkObject == null){
            Debug.LogWarning($"An entity {gameObject.name} should have a NetworkObject component.");
        }
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
        NetworkObject?.Despawn();
        if(NetworkObject == null){
            Destroy(BodyTransform.gameObject);
        }
    }
}