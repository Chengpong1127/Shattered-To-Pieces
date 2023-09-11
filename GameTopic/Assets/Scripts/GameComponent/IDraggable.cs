using UnityEngine;

public interface IAssemblyable{
    public Transform DraggableTransform { get; }
    public Transform FlipTransform { get; }
    public Transform RotationTransform { get; }
    public ulong NetworkObjectID { get; }
    public void SetZRotation(float zRotation);
    public void AddZRotation(float zRotation);
    public void ToggleXScale();
}