using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class EntityTriggerableComponent : BaseCoreComponent, IEntityTriggerable {
    public event Action<Entity> OnTriggerEntity;
    public Collider2D EntityTriggerCollider;

    protected override void Awake() {
        base.Awake();
        ListenTrigger(this.GetCancellationTokenOnDestroy());
        
    }
    private async void ListenTrigger(CancellationToken cancellationToken){
        var trigger = EntityTriggerCollider.GetAsyncTriggerStay2DTrigger();
        Collider2D collider;
        while(!cancellationToken.IsCancellationRequested){

            try{
                collider = await trigger.OnTriggerStay2DAsync(cancellationToken);
            }catch(OperationCanceledException){
                return;
            }
            var entity = collider.GetComponentInParent<Entity>();
            if (entity != null) {
                if (entity is BaseCoreComponent coreComponent && HasTheSameRootWith(coreComponent)) continue;
                OnTriggerEntity?.Invoke(entity);
            }
        }
        
    }
}
