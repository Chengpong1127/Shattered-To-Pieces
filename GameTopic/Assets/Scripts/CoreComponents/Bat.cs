using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : BaseCoreComponent, IRotatable, ITriggerEntity, IAimable
{
    [SerializeField]
    private Transform Handle;
    public Transform RotateBody => BodyTransform;
    public Transform RotateCenter => Handle;

    public Vector2 AimStartPoint => BodyTransform.position;

    public event Action<Entity> OnTriggerEnterEvent;

    public void EndAim(Vector2 aimVector)
    {
        BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        BodyRigidbody.velocity = aimVector * 0.05f;
    }

    public void StartAim(Vector2 aimVector)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var entity = other.GetComponent<Entity>();
        if (entity != null)
        {
            OnTriggerEnterEvent?.Invoke(entity);
        }
    }
}
