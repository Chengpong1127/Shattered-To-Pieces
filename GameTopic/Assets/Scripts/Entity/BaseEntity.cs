using UnityEngine;


public abstract class BaseEntity : MonoBehaviour, IUnit
{
    public virtual int UnitID { get; set; }

    public virtual Transform BodyTransform { get; protected set; }
    protected virtual void Awake()
    {
        BodyTransform = transform;
    }

    protected virtual void Start()
    {
        
    }
}