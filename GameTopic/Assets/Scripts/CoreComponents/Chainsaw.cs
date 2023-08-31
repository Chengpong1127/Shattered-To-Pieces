using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : BaseCoreComponent, IEntityTriggerable {
    public event Action<Entity> OnTriggerEntity;

    private void OnTriggerEnter2D(Collider2D other) {
        var entity = other.GetComponent<Entity>();
        if (entity != null) {
            if (entity is BaseCoreComponent coreComponent && HasTheSameRootWith(coreComponent)) return;
            OnTriggerEntity?.Invoke(entity);
        }
    }

    protected override void Awake() {
        base.Awake();
        AttackDecorator.Instance.Decorate(this);
    }
}
