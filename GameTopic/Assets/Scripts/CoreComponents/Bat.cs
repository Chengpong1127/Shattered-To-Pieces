using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : BaseCoreComponent, IRotatable, ITriggerEntity
{
    [SerializeField]
    private Transform Handle;
    public Transform RotateBody => BodyTransform;
    public Transform RotateCenter => Handle;

    public event Action<Entity> OnTriggerEnterEvent;

    private void OnTriggerEnter2D(Collider2D other) {
        var entity = other.GetComponent<Entity>();
        if (entity != null)
        {
            OnTriggerEnterEvent?.Invoke(entity);
        }
    }
}
