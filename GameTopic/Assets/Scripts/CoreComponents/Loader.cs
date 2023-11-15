using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : BaseCoreComponent, IEntityCollisionable
{
    public event Action<Entity> OnCollisionEntity;

    private void OnTriggerEnter2D(Collider2D other) {
        var entity = other.GetComponent<Entity>();
        if (entity != null) {
            if (entity is BaseCoreComponent coreComponent && HasTheSameRootWith(coreComponent)) return;
            else entity.gameObject.GetComponent<Connector>().Disconnect();
            OnCollisionEntity?.Invoke(entity);
        }
    }
}