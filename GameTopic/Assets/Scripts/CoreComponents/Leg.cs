using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class Leg : BaseCoreComponent , IBodyControlable, IGroundCheckable {
    public BaseCoreComponent body { get; private set; }
    public Collider2D GroundTriggerCollider;

    public bool IsGrounded { get; private set; }

    protected override void Awake() {
        GroundCheck();
        body = this;
        base.Awake();
    }
    private async void GroundCheck() {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        while(cancellationToken.IsCancellationRequested == false){
            await ListenGround(cancellationToken);
            if (cancellationToken.IsCancellationRequested) break;
            await ListenUnground(cancellationToken);
        }
    }
    private async UniTask ListenGround(CancellationToken cancellationToken) {
        var trigger = GroundTriggerCollider.GetAsyncTriggerStay2DTrigger();
        
        while(!IsGrounded && !cancellationToken.IsCancellationRequested) {
            var collider = await trigger.OnTriggerStay2DAsync(cancellationToken);
            if (collider.GetComponentInParent<Taggable>()?.HasTag("Ground") ?? false) {
                IsGrounded = true;
            }
        }
    }
    private async UniTask ListenUnground(CancellationToken cancellationToken) {
        var trigger = GroundTriggerCollider.GetAsyncTriggerExit2DTrigger();
        await trigger.OnTriggerExit2DAsync(cancellationToken);
        IsGrounded = false;
    }
}
