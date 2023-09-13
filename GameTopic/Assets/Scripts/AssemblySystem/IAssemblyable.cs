using UnityEngine;

public interface IAssemblyable{
    public Transform DraggableTransform { get; }
    public Transform AssemblyTransform { get; }
    public ulong NetworkObjectID { get; }
}