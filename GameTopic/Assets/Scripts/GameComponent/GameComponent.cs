using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Netcode.Components;

public class GameComponent : MonoBehaviour, IGameComponent
{
    public Transform BodyTransform => bodyTransform;

    public Rigidbody2D BodyRigidbody => bodyRigidbody;

    public Collider2D BodyCollider => bodyCollider;
    public int UnitID { get; set; }
    public ITreeNode Parent { get; private set; } = null;
    public IList<ITreeNode> Children { get; private set; } = new List<ITreeNode>();

    public IConnector Connector => connector;
    public BaseCoreComponent CoreComponent => coreComponent;
    public Transform DraggableTransform => bodyTransform;
    public Animator BodyAnimator => bodyAnimator;
    public string ComponentName { get; set; }

    public NetworkObject BodyNetworkObject => bodyNetworkObject;
    private NetworkObject bodyNetworkObject;
    private float zRotation = 0;

    #region Inspector

    [Header("References Setting")]
    [Tooltip("The main transform of the body of the game component.")]
    [SerializeField]
    private Transform bodyTransform;
    [Tooltip("The main rigidbody of the body of the game component.")]
    [SerializeField]
    private Rigidbody2D bodyRigidbody;
    [Tooltip("The main collider of the body of the game component.")]
    [SerializeField]
    private Collider2D bodyCollider;
    [Tooltip("The connector of the game component.")]
    [SerializeField]
    private IConnector connector;
    [Tooltip("The core component of the game component.")]
    [SerializeField]
    private BaseCoreComponent coreComponent;
    [Tooltip("The animator of the game component.")]
    [SerializeField]
    private Animator bodyAnimator;

    #endregion

    public void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info)
    {
        if (parentComponent == null) throw new ArgumentNullException("parentComponent");
        if (info == null) throw new ArgumentNullException("info");
        connector.ConnectToComponent(parentComponent.Connector, info);
        Parent = parentComponent;
        Parent.Children.Add(this);
        BodyRigidbody.isKinematic = true;
        BodyCollider.isTrigger = true;
    }

    public void DisconnectFromParent()
    {
        if (Parent == null) return;
        Parent.Children.Remove(this);
        Parent = null;
        connector.Disconnect();
        BodyRigidbody.isKinematic = false;
        BodyCollider.isTrigger = false;
    }

    public IInfo Dump(){
        var info = new GameComponentInfo
        {
            ComponentName = ComponentName,
            ConnectionInfo = connector.Dump() as ConnectionInfo,
            ConnectionZRotation = BodyTransform.rotation.eulerAngles.z,
            ToggleXScale = BodyTransform.localScale.x < 0,
        };
        return info;
    }
    public void Load(IInfo info)
    {
        if (info is not GameComponentInfo){
            throw new ArgumentException("info is not GameComponentInfo");
        }
        var componentInfo = info as GameComponentInfo;
        ComponentName = componentInfo.ComponentName;
        zRotation = componentInfo.ConnectionZRotation;
    }

    public (IGameComponent, ConnectionInfo) GetAvailableConnection(){
        var (availableParent, targetID) = connector.GetAvailableConnector();
        if (availableParent == null){
            return (null, ConnectionInfo.NoConnection());
        }
        Debug.Assert(availableParent.GameComponent != null);

        var tempTree = new Tree(this);
        var result = false;
        tempTree.TraverseBFS((node) => {
            if (node == availableParent.GameComponent){
                result = true;
            }
        });
        if (result){
            return (null, ConnectionInfo.NoConnection());
        }

        var newInfo = new ConnectionInfo{
            linkedTargetID = targetID,
        };
        return (availableParent.GameComponent, newInfo);
    }
    public void SetDragging(bool dragging){
        
        switch (dragging){
            case true:
                connector.SetNonConnectedTargetsDisplay(false);
                BodyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                BodyCollider.enabled = false;
                break;
            case false:
                connector.SetNonConnectedTargetsDisplay(true);
                BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                BodyCollider.enabled = true;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                break;
        }
    }
    public void SetAvailableForConnection(bool available){
        switch(available){
            case true:
                connector.SetNonConnectedTargetsDisplay(true);
                break;
            case false:
                connector.SetAllTargetDisplay(false);
                break;
        }
        
    }

    private void Awake()
    {
        bodyTransform ??= transform ?? throw new ArgumentNullException(nameof(bodyTransform));
        bodyRigidbody ??= GetComponent<Rigidbody2D>() ?? throw new ArgumentNullException(nameof(bodyRigidbody));
        bodyCollider ??= GetComponent<Collider2D>() ?? throw new ArgumentNullException(nameof(bodyCollider));
        connector ??= GetComponentInChildren<IConnector>() ?? throw new ArgumentNullException(nameof(connector));
        coreComponent ??= GetComponentInChildren<BaseCoreComponent>() ?? throw new ArgumentNullException(nameof(coreComponent));
        bodyNetworkObject ??= GetComponent<NetworkObject>() ?? throw new ArgumentNullException(nameof(bodyNetworkObject));
        bodyAnimator ??= GetComponent<Animator>();

        coreComponent.OwnerGameComponent = this;
        
        DisconnectFromParent();

    }
    public void SetZRotation(float newZRotation)
    {
        zRotation = newZRotation;
        bodyTransform.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public void AddZRotation(float newZRotation)
    {
        zRotation += newZRotation;
        SetZRotation(zRotation);
    }

    public void ToggleXScale()
    {
        bodyTransform.localScale = new Vector3(-bodyTransform.localScale.x, bodyTransform.localScale.y, bodyTransform.localScale.z);
    }
    

    public ITreeNode GetRoot()
    {
        var parent = Parent;
        if (parent == null){
            return this;
        }
        return parent.GetRoot();
    }

    public void DisconnectAllChildren()
    {
        while(Children.Count > 0){
            var child = Children[0] as GameComponent;
            child.DisconnectFromParent();
        }
    }
}
