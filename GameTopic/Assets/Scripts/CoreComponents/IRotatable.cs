using UnityEngine;
public interface IRotatable {
    public Transform RotateBody { get; }
    public Transform RotateCenter { get; }
}