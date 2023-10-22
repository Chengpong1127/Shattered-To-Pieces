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
    public Transform AssemblyTransform => BodyTransform;
    public bool CanSelected = true;
    public bool IsSelected { get; private set; } = false;

    public bool HaveConnected=false;
    #region Inspector
    [Tooltip("The connector of the game component.")]
    [SerializeField]
    private IConnector connector;
    

    #endregion

    public virtual void ConnectToParent(IGameComponent parentComponent, ConnectionInfo info)
    {
        if (parentComponent == null) throw new ArgumentNullException("parentComponent");
        if (info == null) throw new ArgumentNullException("info");
        var parent = parentComponent as GameComponent;

        Parent = parent;
        Parent.Children.Add(this);
        BodyTransform.position = parentComponent.BodyTransform.position;
        connector.ConnectToComponent(parent.Connector, info);
        NetworkObject.ChangeOwnership(parent.NetworkObject.OwnerClientId);

        (GetRoot() as GameComponent)?.OnRootConnectionChanged?.Invoke();
        GameEvents.GameComponentEvents.OnGameComponentConnected.Invoke(this, Parent as GameComponent);
    }

    public virtual void DisconnectFromParent()
    {
        if (Parent == null) return;
        var root = GetRoot() as GameComponent;
        Parent.Children.Remove(this);
        Parent = null;
        connector.Disconnect();

        GameEvents.GameComponentEvents.OnGameComponentDisconnected.Invoke(this, Parent as GameComponent);
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
    public void SetSelected(bool selected){
        SetSelectedClientRpc(selected);
        IsSelected = selected;
    }
    [ClientRpc]
    private void SetSelectedClientRpc(bool selected){
        switch (selected){
            case true:
                BodyColliders.ToList().ForEach((collider) => collider.isTrigger = true);
                BodyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                GameEvents.GameComponentEvents.OnGameComponentSelected.Invoke(this, true);
                break;
            case false:
                BodyColliders.ToList().ForEach((collider) => collider.isTrigger = false);
                BodyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                BodyRigidbody.angularVelocity = 0;
                BodyRigidbody.velocity = Vector2.zero;
                GameEvents.GameComponentEvents.OnGameComponentSelected.Invoke(this, false);
                break;
        }
    }
    [ClientRpc]
    public virtual void SetAvailableForConnection_ClientRpc(bool available, ulong displayClientID){
        if (displayClientID == Utils.GetLocalPlayer().OwnerClientId){
            switch(available){
                case true:
                    connector.SetNonConnectedTargetsDisplay(true);
                    break;
                case false:
                    connector.SetAllTargetDisplay(false);
                    break;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        connector ??= GetComponent<IConnector>() ?? throw new ArgumentNullException(nameof(connector));
        connector.OnJointBreak += JointBreakHandler;
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
    protected void JointBreakHandler(){
        if (IsOwner){
            JointBreakHandler_ServerRpc();
        }
    }
    [ServerRpc]
    protected void JointBreakHandler_ServerRpc(){
        DisconnectFromParent();
    }
}
