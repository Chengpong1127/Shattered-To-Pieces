using UnityEngine;

public interface IAssemblyable{
    public Transform DraggableTransform { get; }
    public Transform FlipTransform { get; }
    public Transform RotationTransform { get; }
    public ulong NetworkObjectID { get; }
}