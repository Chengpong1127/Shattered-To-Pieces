using UnityEngine;

public interface IAimable{
    public Vector2 AimStartPoint { get; }
    public void StartAim(Vector2 aimVector);
    public void EndAim(Vector2 aimVector);
}