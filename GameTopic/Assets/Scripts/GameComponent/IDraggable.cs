using UnityEngine;

public interface IDraggable{
    public Transform DraggableTransform { get; }
    public ulong NetworkObjectID { get; }
    public void SetZRotation(float zRotation);
    public void AddZRotation(float zRotation);
    public void ToggleXScale();
}