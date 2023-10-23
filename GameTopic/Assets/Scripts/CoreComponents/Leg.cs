using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;

public class Leg : BaseCoreComponent , IBodyControlable, IGroundCheckable, IMovable {
    public BaseCoreComponent body { get; private set; }
    public Collider2D GroundTriggerCollider;
    private NetworkVariable<float> Speed = new NetworkVariable<float>(0);
    public bool IsGrounded { get; private set; }

    protected override void Awake() {
        GroundCheck();
        body = this;
        base.Awake();
    }
    private async void GroundCheck() {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        while(cancellationToken.IsCancellationRequested == false){
            try{
                await ListenGround(cancellationToken);
                await ListenUnground(cancellationToken);
            }catch(OperationCanceledException){
                return;
            }
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

    private void FixedUpdate() {
        if (IsOwner) {
            Move();
        }
    }
    private void Move(){
        if (Speed.Value != 0) {
            BodyRigidbody.velocity = new Vector2(Speed.Value, BodyRigidbody.velocity.y);
        }
    }

    public void StartMove(float speed)
    {
        Speed.Value += speed;
    }

    public void StopMove(float speed)
    {
        Speed.Value -= speed;
    }

}
