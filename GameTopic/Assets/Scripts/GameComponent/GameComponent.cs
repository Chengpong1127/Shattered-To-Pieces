using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(Connector))]
public class GameComponent : AbilityEntity, IGameComponent
{
    /// <summary>
    /// This event will be invoked when the the children of a root game component is changed. Only root will invoke this event.
    /// </summary>
    public event Action OnRootConnectionChanged;
    public ITreeNode Parent { get; private set; } = null;
    public IList<ITreeNode> Children { get; private set; } = new List<ITreeNode>();

    public IConnector Connector => connector;
    public Transform DraggableTransform => BodyTransform;
    public string ComponentName { get; set; }
    public Transform AssemblyTransform => assemblyTransform;


    #region Inspector

    [SerializeField]
    private Transform assemblyTransform;
    [Tooltip("The connector of the game component.")]
    [SerializeField]
    private IConnector connector;
    

    #endregion

    public virtual void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info)
    {
        if (parentComponent == null) throw new ArgumentNullException("parentComponent");
        if (info == null) throw new ArgumentNullException("info");
        Parent = parentComponent;
        Parent.Children.Add(this);
        BodyColliders.ToList().ForEach((collider) => collider.isTrigger = false);
        connector.ConnectToComponent(parentComponent.Connector, info);
        (GetRoot() as GameComponent)?.OnRootConnectionChanged?.Invoke();
    }

    public virtual void DisconnectFromParent()
    {
        if (Parent == null) return;
        var root = GetRoot() as GameComponent;
        Parent.Children.Remove(this);
        Parent = null;
        connector.Disconnect();
        BodyColliders.ToList().ForEach((collider) => collider.isTrigger = false);
        root?.OnRootConnectionChanged?.Invoke();
    }

    public IInfo Dump(){
        var info = new GameComponentInfo
        {
            ComponentName = ComponentName,
            ConnectionInfo = connector.Dump() as ConnectionInfo,
            ConnectionZRotation = AssemblyTransform.rotation.eulerAngles.z,
            ToggleXScale = AssemblyTransform.localScale.x < 0,
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
        AssemblyTransform.rotation = Quaternion.Euler(0, 0, componentInfo.ConnectionZRotation);
        AssemblyTransform.localScale = new Vector3(componentInfo.ToggleXScale ? -1 : 1, 1, 1);
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
    public void SetSelected(bool selected){
        switch (selected){
            case true:
                SetSelectedClientRpc(true);
                BodyColliders.ToList().ForEach((collider) => collider.isTrigger = true);
                BodyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                break;
            case false:
                SetSelectedClientRpc(false);
                BodyColliders.ToList().ForEach((collider) => collider.isTrigger = false);
                BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                break;
        }
    }
    [ClientRpc]
    private void SetSelectedClientRpc(bool selected){
        switch (selected){
            case true:
                connector.SetNonConnectedTargetsDisplay(false);
                BodyColliders.ToList().ForEach((collider) => collider.isTrigger = true);
                BodyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                GameEvents.GameComponentEvents.OnGameComponentSelected.Invoke(this, true);
                break;
            case false:
                connector.SetNonConnectedTargetsDisplay(true);
                BodyColliders.ToList().ForEach((collider) => collider.isTrigger = false);
                BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                GameEvents.GameComponentEvents.OnGameComponentSelected.Invoke(this, false);
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
        connector ??= GetComponent<IConnector>() ?? throw new ArgumentNullException(nameof(connector));
        if (assemblyTransform == null) Debug.LogWarning("The assemblyTransform is not set.");
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
