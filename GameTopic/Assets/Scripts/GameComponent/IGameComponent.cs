using UnityEngine;
using Unity.Netcode;
using System;
// Description: Defines the basic interfaces for the game components.
public interface IGameComponent: ITreeNode, IAssemblyable
{
    public Transform BodyTransform { get; }

    public Rigidbody2D BodyRigidbody { get; }

    public Collider2D[] BodyColliders { get; }

    public Animator BodyAnimator { get; }
    public ulong NetworkObjectId { get; }
    public event Action OnRootConnectionChanged;
    public string ComponentName { get; set; }
    /// <summary>
    /// The Connector of the game component.
    /// </summary>
    /// <value></value>
    public IConnector Connector { get; }
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
    /// Make all children disconnect from this component.
    /// </summary>
    public void DisconnectAllChildren();
    public void SetSelected(bool assemblyMode);
    public void Die();
}


