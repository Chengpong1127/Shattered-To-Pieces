using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterCtrl {
    public bool Landing { get; }
    public void Move(Vector3 Motion, ForceMode2D Mode);
    public void Push(Vector3 Motion);
    public void Bondage();
}