using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;

public class Leg : BaseCoreComponent, IGroundCheckable, IMovable, IForceAddable {
    private NetworkVariable<float> _Speed = new NetworkVariable<float>(0);
    bool SetAnimatorSwitch = false;
    private float Speed {
        get { return _Speed.Value; }
        set {
            _Speed.Value = value;
            if (SetAnimatorSwitch) { return; }
            SetAnimatorSwitch = true;
            Invoke("SetAnimatorValue", 0.1f);
        }
    }
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
        if (Speed != 0) {
            BodyRigidbody.velocity = new Vector2(Speed, BodyRigidbody.velocity.y);
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
            Speed = direction == MoveDirection.Left ? -MovingSpeed : MovingSpeed;
            return;
        }
        else if (CurrentDirection == direction) return;
        else if (CurrentDirection != direction){
            CurrentDirection = MoveDirection.Both;
            Speed = 0;
            return;
        }
    }

    public void StopMoveDirection(MoveDirection direction)
    {
        if(CurrentDirection == MoveDirection.Both) {
            CurrentDirection = null;
            if(direction == MoveDirection.Left) { SetMoveDirection(MoveDirection.Right); }
            else { SetMoveDirection(MoveDirection.Left); }
        } else if (CurrentDirection == direction || direction == MoveDirection.Both){
            CurrentDirection = null;
            Speed = 0;
        }
    }

    void SetAnimatorValue() {
        BodyAnimator.SetFloat("Speed", Math.Abs(Speed));
        SetAnimatorSwitch = false;
    }
}
