using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class EntityCollisionableComponent : BaseCoreComponent, IEntityCollisionable
{
    public event Action<Entity> OnCollisionEntity;
    public Collider2D EntityTriggerCollider;

    protected override void Awake() {
        base.Awake();
        ListenTrigger(this.GetCancellationTokenOnDestroy());
        
    }
    private async void ListenTrigger(CancellationToken cancellationToken){
        var trigger = EntityTriggerCollider.GetAsyncCollisionEnter2DTrigger();
        Collision2D collider;
        while(!cancellationToken.IsCancellationRequested){

            try{
                collider = await trigger.OnCollisionEnter2DAsync(cancellationToken);
            }catch(OperationCanceledException){
                return;
            }
            var entity = collider.gameObject.GetComponentInParent<Entity>();
            if (entity != null) {
                if (entity is BaseCoreComponent coreComponent && HasTheSameRootWith(coreComponent)) continue;
                OnCollisionEntity?.Invoke(entity);
            }
        }
        
    }
}
