using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;

public class Leg : BaseCoreComponent, IGroundCheckable, IMovable, IForceAddable {
    private NetworkVariable<float> Speed = new NetworkVariable<float>(0);
    public float MovingSpeed;
    private MoveDirection? CurrentDirection = null;

    public GroundDetector GroundDetector;
    public bool IsGrounded => GroundDetector.IsGrounded;

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

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        AddForce_ClientRpc(force, mode);
    }
    [ClientRpc]
    private void AddForce_ClientRpc(Vector2 force, ForceMode2D mode)
    {
        if (IsOwner)
        {
            BodyRigidbody.AddForce(force, mode);
        }
    }

    public void SetMovingSpeed(float speed)
    {
        MovingSpeed = speed;
    }

    public void SetMoveDirection(MoveDirection direction)
    {
        if (CurrentDirection == null){
            CurrentDirection = direction;
            Speed.Value = direction == MoveDirection.Left ? -MovingSpeed : MovingSpeed;
            return;
        }
        else if (CurrentDirection == direction) return;
        else if (CurrentDirection != direction){
            CurrentDirection = null;
            Speed.Value = 0;
            return;
        }
    }

    public void StopMoveDirection(MoveDirection direction)
    {
        if (CurrentDirection == direction){
            CurrentDirection = null;
            Speed.Value = 0;
        }
    }
}
