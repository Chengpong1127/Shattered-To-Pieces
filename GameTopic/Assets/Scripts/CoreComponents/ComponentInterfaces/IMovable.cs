using UnityEngine;
public interface IMovable{
    public void SetMovingSpeed(float speed);
    public void SetMoveDirection(MoveDirection direction);
    public void StopMoveDirection(MoveDirection direction);
}
public enum MoveDirection{
    Left,
    Right,
}