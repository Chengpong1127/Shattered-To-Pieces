using UnityEngine;


public abstract class BaseEntity : MonoBehaviour, IUnit
{
    public virtual int UnitID { get; set; }

    public virtual Transform BodyTransform { get; protected set; }
    public virtual Rigidbody2D BodyRigidbody { get; protected set; }
    public virtual Collider2D BodyCollider { get; protected set; }
    protected virtual void Awake()
    {
        BodyTransform = transform;
        BodyRigidbody = GetComponent<Rigidbody2D>();
        BodyCollider = GetComponent<Collider2D>();
    }

    protected virtual void Start()
    {
        
    }
}