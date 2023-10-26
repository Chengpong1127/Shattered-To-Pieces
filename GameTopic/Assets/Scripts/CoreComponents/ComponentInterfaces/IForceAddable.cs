using UnityEngine;

public interface IForceAddable {
    public void AddForce(Vector2 force, ForceMode2D mode);
}