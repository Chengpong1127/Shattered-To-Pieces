using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameComponent : MonoBehaviour, IGameComponent
{
    public Transform BodyTransform => bodyTransform;

    public Rigidbody2D BodyRigidbody => bodyRigidbody;

    public Collider2D BodyCollider => bodyCollider;
    public int UnitID { get; set; }
    

    public IConnector Connector => connector;
    public ICoreComponent CoreComponent => coreComponent;
    public Transform DragableTransform => bodyTransform;
    public string ComponentName { get; set; }
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
    private ICoreComponent coreComponent;

    #endregion

    public void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info)
    {
        Debug.Assert(parentComponent != null);
        Debug.Assert(connector != null);
        Debug.Assert(parentComponent.Connector != null);
        connector.ConnectToComponent(parentComponent.Connector, info);
        
    }

    public void DisconnectFromParent()
    {
        connector.Disconnect();
        
    }

    public IInfo Dump(){
        var info = new GameComponentInfo();
        info.ComponentName = ComponentName;
        info.ConnectionInfo = connector.Dump() as ConnectionInfo;
        info.ConnectionZRotation = zRotation;
        return info;
    }
    public void Load(IInfo info)
    {
        Debug.Assert(info is GameComponentInfo);
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

        // check availableParent cannot be one of the children of this component
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

    public ITreeNode GetParent(){
        var parentConnector = connector.GetParentConnector();
        if (parentConnector == null){
            return null;
        }
        return parentConnector.GameComponent as ITreeNode;
    }
    public IList<ITreeNode> GetChildren(){
        var childConnectors = connector.GetChildConnectors();
        var children = new List<ITreeNode>();
        foreach (var childConnector in childConnectors){
            if (childConnector == null){
                continue;
            }
            children.Add(childConnector.GameComponent as ITreeNode);
        }
        return children;
    }
    public void SetDragging(bool dragging){
        Debug.Assert(connector != null);
        connector.SetSelectingMode(dragging);
        BodyRigidbody.angularVelocity = 0;
        switch(dragging){
            case true:
                BodyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                BodyCollider.enabled = false;
                break;
            case false:
                BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                BodyCollider.enabled = true;
                break;
        }
    }
    public void SetAvailableForConnection(bool available){
        Debug.Assert(connector != null);
        switch(available){
            case true:
                break;
            case false:
                break;
        }
        connector.SetConnectMode(available);
    }

    private void Awake()
    {
        if (bodyTransform == null)
        {
            bodyTransform = transform;
            Debug.Assert(bodyTransform != null, "The body transform is not set.");
        }
        if (bodyRigidbody == null)
        {
            bodyRigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(bodyRigidbody != null, "The body rigidbody is not set.");
        }
        if (bodyCollider == null)
        {
            bodyCollider = GetComponentInChildren<Collider2D>();
            Debug.Assert(bodyCollider != null, "The body collider is not set.");
        }
        if (connector == null)
        {
            connector = GetComponentInChildren<IConnector>();
            Debug.Assert(connector != null, "The connector is not set.");
        }
        if (coreComponent == null)
        {
            coreComponent = GetComponentInChildren<ICoreComponent>();
            if (coreComponent == null)
            {
                Debug.LogWarning("The core component is not set.");
            }
        }

        DisconnectFromParent();
        coreComponent.OwnerGameComponent = this;
    }
    public void SetZRotation()
    {
        SetZRotation(zRotation);
    }
    public void SetZRotation(float newZRotation)
    {
        zRotation = newZRotation;
        bodyTransform.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public void AddZRotation(float newzRotation)
    {
        zRotation += newzRotation;
        SetZRotation(zRotation);
    }
}
