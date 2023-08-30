using System;
using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Authoring;
using UnityEngine;

public class Bat : BaseCoreComponent, IRotatable, IEntityTriggerable
{
    [SerializeField]
    private AttributeScriptableObject AttackPointAttribute; 
    [SerializeField]
    private Transform Handle;
    public Transform RotateBody => BodyTransform;
    public Transform RotateCenter => Handle;

    public event Action<Entity> OnTriggerEntity;

    private void OnTriggerEnter2D(Collider2D other) {
        var entity = other.GetComponent<Entity>();
        if (entity != null)
        {
            if (entity is BaseCoreComponent coreComponent && HasTheSameRootWith(coreComponent)) return;
            OnTriggerEntity?.Invoke(entity);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        AttackDecorator.Instance.Decorate(this);
    }
}
