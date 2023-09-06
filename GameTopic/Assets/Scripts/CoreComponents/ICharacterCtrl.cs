using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterCtrl {
    public bool Landing { get; }
    public void Move(Vector2 Motion);
    public void VerticalMove(float Motion);
    public void HorizontalMove(float Motion);
    public void AddForce(Vector2 Motion, ForceMode2D Mode);
    public void Push(Vector2 Motion);
    public void Bondage();
}