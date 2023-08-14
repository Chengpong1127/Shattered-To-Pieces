using UnityEngine;
// Description: Defines the basic interfaces for the game components.
public interface IGameComponent: ITreeNode, IUnit, IDraggable
{
    public Transform BodyTransform { get; }

    public Rigidbody2D BodyRigidbody { get; }

    public Collider2D BodyCollider { get; }

    /// <summary>
    /// Whether the game component is in the device. // TODO
    /// </summary>
    /// <value></value>
    public string ComponentName { get; set; }
    /// <summary>
    /// The Connector of the game component.
    /// </summary>
    /// <value></value>
    public IConnector Connector { get; }
    /// <summary>
    /// The core component of the game component.
    /// </summary>
    /// <value></value>
    public ICoreComponent CoreComponent { get; }
    /// <summary>
    /// Connect to a parent component.
    /// </summary>
    /// <param name="parentComponent"> The parent component. </param>
    /// <param name="info"> The connection info. </param>
    public void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info);
    /// <summary>
    /// Disconnect from the parent component.
    /// </summary>
    public void DisconnectFromParent();
    /// <summary>
    /// Get the first available connection.
    /// </summary>
    /// <returns> The first available connection. </returns>
    public (IGameComponent, ConnectionInfo) GetAvailableConnection();
    public void SetDragging(bool assemblyMode);
    public void SetAvailableForConnection(bool draggingMode);

    public void SetZRotation(float zRotation);
    public void AddZRotation(float zRotation);
}


