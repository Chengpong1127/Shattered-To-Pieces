using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterCtrl {
    public bool Landing { get; }
    public void Move(Vector2 Motion);
    public void Fly(Vector2 Motion);
    public void Push(Vector2 Motion);
    public void Bondage();
}