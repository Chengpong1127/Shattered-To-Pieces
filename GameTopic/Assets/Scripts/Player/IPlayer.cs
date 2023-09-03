using UnityEngine;


public interface IPlayer: ICameraTraceable{
    public void SetPlayerPoint(Transform transform);
    public bool IsLoaded { get; }
    public Device SelfDevice { get; }
}