using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class GameComponent : AbilityEntity, IGameComponent
{
    public ITreeNode Parent { get; private set; } = null;
    public IList<ITreeNode> Children { get; private set; } = new List<ITreeNode>();

    public IConnector Connector => connector;
    public Transform DraggableTransform => BodyTransform;
    public string ComponentName { get; set; }


    public ulong NetworkObjectID => NetworkObject.NetworkObjectId;

    public Transform FlipTransform => flipTransform;

    public Transform RotationTransform => rotationTransform;

    private float zRotation = 0;

    #region Inspector

    [SerializeField]
    private Transform flipTransform;
    [SerializeField]
    private Transform rotationTransform;
    [Tooltip("The connector of the game component.")]
    [SerializeField]
    private IConnector connector;
    

    #endregion

    public void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info)
    {
        if (parentComponent == null) throw new ArgumentNullException("parentComponent");
        if (info == null) throw new ArgumentNullException("info");
        connector.ConnectToComponent(parentComponent.Connector, info);
        Parent = parentComponent;
        Parent.Children.Add(this);
        BodyRigidbody.isKinematic = true;
        BodyColliders.ToList().ForEach((collider) => collider.isTrigger = true);
    }

    public void DisconnectFromParent()
    {
        if (Parent == null) return;
        Parent.Children.Remove(this);
        Parent = null;
        connector.Disconnect();
        BodyRigidbody.isKinematic = false;
        BodyColliders.ToList().ForEach((collider) => collider.isTrigger = false);
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
    [ClientRpc]
    public void SetDraggingClientRpc(bool dragging){
        
        switch (dragging){
            case true:
                connector.SetNonConnectedTargetsDisplay(false);
                BodyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                BodyColliders.ToList().ForEach((collider) => collider.enabled = false);
                break;
            case false:
                connector.SetNonConnectedTargetsDisplay(true);
                BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                BodyColliders.ToList().ForEach((collider) => collider.enabled = true);
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                break;
        }
    }
    [ClientRpc]
    public void SetAvailableForConnectionClientRpc(bool available){
        switch(available){
            case true:
                connector.SetNonConnectedTargetsDisplay(true);
                break;
            case false:
                connector.SetAllTargetDisplay(false);
                break;
        }
        
    }

    protected override void Awake()
    {
        base.Awake();
        connector ??= GetComponentInChildren<IConnector>() ?? throw new ArgumentNullException(nameof(connector));
        if (flipTransform == null) Debug.LogWarning("The flip transform is not set.");
        if (rotationTransform == null) Debug.LogWarning("The rotation transform is not set.");


        
        DisconnectFromParent();

    }
    public void SetZRotation(float newZRotation)
    {
        zRotation = newZRotation;
        rotationTransform.rotation = Quaternion.Euler(0, 0, zRotation);
    }

    public void AddZRotation(float newZRotation)
    {
        zRotation += newZRotation;
        SetZRotation(zRotation);
    }

    public void ToggleXScale()
    {
        flipTransform.localScale = new Vector3(-flipTransform.localScale.x, flipTransform.localScale.y, flipTransform.localScale.z);
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

    public override void OnDestroy()
    {
        DisconnectAllChildren();
        DisconnectFromParent();
    }
}
