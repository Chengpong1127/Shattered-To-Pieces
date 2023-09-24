using UnityEngine.InputSystem;
using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Cysharp.Threading.Tasks;
using System.Linq;
public class AssemblyController : NetworkBehaviour
{
    private Func<ulong[]> GetSelectableGameObject { get; set; }
    private Func<ulong[]> GetConnectableGameObject { get; set; }
    public float RotationUnit { get; private set; }
    private ulong[] tempConnectableComponentIDs;

    private ulong? SelectedComponentID { get; set; }
    private ulong? tempSelectedParentComponentID { get; set; }
    private ConnectionInfo tempConnectionInfo { get; set; }

    /// <summary>
    /// This event will be invoked when a game component is started to drag.
    /// </summary>
    public event Action<IGameComponent> OnGameComponentDraggedStart;
    /// <summary>
    /// This event will be invoked after a game component is dragged and released.
    /// </summary>
    public event Action<IGameComponent> OnGameComponentDraggedEnd;
    /// <summary>
    /// This event will be invoked after a game component is connected to another game component.
    /// </summary>
    public event Action<IGameComponent> AfterGameComponentConnected;
    private InputAction selectAction;
    private InputAction disconnectAction;
    private InputAction flipAction;
    private InputAction rotateAction;

    public void ServerInitialize(
        Func<ulong[]> getSelectableGameObjectIDs, 
        Func<ulong[]> getConnectableGameObjectIDs, 
        float rotationUnit = 0.3f
    ){
        if (IsServer){
            GetSelectableGameObject = getSelectableGameObjectIDs;
            GetConnectableGameObject = getConnectableGameObjectIDs;
            RotationUnit = rotationUnit;
        }
    }
    public void OwnerInitialize(
        InputAction selectAction, 
        InputAction DisconnectAction,
        InputAction flipAction, 
        InputAction rotateAction
        ){
        if (IsOwner){
            this.selectAction = selectAction;
            this.disconnectAction = DisconnectAction;
            this.flipAction = flipAction;
            this.rotateAction = rotateAction;
            this.selectAction.started += SelectHandler;
            this.disconnectAction.started += DisconnectHandler;
            this.flipAction.started += FlipHandler;
            this.rotateAction.started += RotateHandler;
            this.selectAction.Disable();
            this.disconnectAction.Disable();
            this.flipAction.Disable();
            this.rotateAction.Disable();
        }
    }
    void OnEnable()
    {
        if(IsOwner){
            selectAction.Enable();
            disconnectAction.Enable();
            flipAction.Enable();
            rotateAction.Enable();
        }
    }
    void OnDisable()
    {
        if(IsOwner){
            selectAction.Disable();
            disconnectAction.Disable();
            flipAction.Disable();
            rotateAction.Disable();
            CancelLastSelection();
        }
    }
    private void SelectHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var targets = Utils.GetGameObjectsUnderMouse<Target>();
            if (targets.Length > 0){
                SelectTargetHandler(targets.First());
                return;
            }
            var components = Utils.GetGameObjectsUnderMouse<IGameComponent>();
            if (components.Length > 0){
                var orderedComponents = components
                    .Where(c => GetSelectableGameObject().Contains(c.NetworkObjectID))
                    .OrderBy(c => c.BodyTransform.position.z);
                if (orderedComponents.Count() > 0){
                    if (orderedComponents.First().NetworkObjectID != SelectedComponentID){
                        SelectComponentHandler_ServerRpc(orderedComponents.First().NetworkObjectID);
                    }
                }
            }
        }
    }
    [ServerRpc]
    private void SelectComponentHandler_ServerRpc(ulong componentID){
        var gameComponent = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<IGameComponent>();
        CancelLastSelection();
        tempSelectedParentComponentID = gameComponent.Parent == null ? null : (gameComponent.Parent as IGameComponent).NetworkObjectID;
        tempConnectionInfo = gameComponent.Parent == null ? null : (gameComponent.Connector.Dump() as ConnectionInfo);
        gameComponent.DisconnectFromParent();
        gameComponent.DisconnectAllChildren();
        SelectedComponentID = gameComponent.NetworkObjectID;
        SetSelect_ServerRpc(SelectedComponentID.Value, true);
        tempConnectableComponentIDs = GetConnectableGameObject();
        SetAvailableForConnection(tempConnectableComponentIDs, true);
    }
    private void CancelLastSelection(){
        if (SelectedComponentID.HasValue){
            SetSelect_ServerRpc(SelectedComponentID.Value, false);
            if (tempSelectedParentComponentID.HasValue){
                Connection_ServerRpc(SelectedComponentID.Value, tempSelectedParentComponentID.Value, tempConnectionInfo.linkedTargetID);
                tempSelectedParentComponentID = null;
                tempConnectionInfo = null;
            }
            SelectedComponentID = null;
        }
    }
    private void SelectTargetHandler(Target target){
        if (IsOwner){
            if (SelectedComponentID.HasValue){
                Connection_ServerRpc(SelectedComponentID.Value, target.OwnerConnector.GameComponent.NetworkObjectID, target.TargetID);
                SetSelect_ServerRpc(SelectedComponentID.Value, false);
                SelectedComponentID = null;
            }
        }
    }
    [ServerRpc]
    private void Connection_ServerRpc(ulong componentID, ulong parentComponentID, int targetID){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
        var parentComponent = Utils.GetLocalGameObjectByNetworkID(parentComponentID)?.GetComponent<IGameComponent>();
        var connectionInfo = new ConnectionInfo
        {
            linkedTargetID = targetID,
        };

        component.ConnectToParent(parentComponent, connectionInfo);
        SetAvailableForConnection(tempConnectableComponentIDs, false);
        tempConnectableComponentIDs = null;
    }
    [ServerRpc]
    private void SetSelect_ServerRpc(ulong componentID, bool selected){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.SetSelected(selected);
    }



    private void DisconnectHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var components = Utils.GetGameObjectsUnderMouse<IGameComponent>();
            var orderedComponents = components
                    .Where(c => GetSelectableGameObject().Contains(c.NetworkObjectID))
                    .OrderBy(c => c.BodyTransform.position.z);
            if (orderedComponents.Count() > 0){
                DisconnectServerRpc(orderedComponents.First().NetworkObjectID);
            }
        }
    }
    [ServerRpc]
    private void DisconnectServerRpc(ulong componentID){
        CancelLastSelection();
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
        Debug.Assert(component != null, "component is null");
        component.DisconnectFromParent();
        component.DisconnectAllChildren();
    }

    #region Flip
    private void FlipHandler(InputAction.CallbackContext context){
        if (IsOwner){
            if (SelectedComponentID.HasValue){
                Flip_ServerRpc(SelectedComponentID.Value);
            }
        }

    }
    [ServerRpc]
    private void Flip_ServerRpc(ulong componentID){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IAssemblyable>();
        Debug.Assert(component != null, "component is null");
        component.AssemblyTransform.localScale = new Vector3(-component.AssemblyTransform.localScale.x, component.AssemblyTransform.localScale.y, component.AssemblyTransform.localScale.z);
    }
    #endregion
    #region Rotate
    private void RotateHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var value = context.ReadValue<float>();
            if (SelectedComponentID.HasValue){
                AddRotation(SelectedComponentID.Value, value * RotationUnit);
            }
            
        }
    }
    private void AddRotation(ulong componentID, float rotation){
        var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IAssemblyable>();
        Debug.Assert(component != null, "component is null");
        component.AssemblyTransform.Rotate(new Vector3(0, 0, rotation));
    }
    #endregion
    private async void HandleComponentDraggedStart(ulong draggableID)
    {
        if(IsOwner){
            HandleComponentDraggedStartServerRpc(draggableID);
            await UniTask.DelayFrame(2);
            ChangeOwnership_ServerRpc(draggableID);
        }
    }
    [ServerRpc]
    private void HandleComponentDraggedStartServerRpc(ulong draggableID){
        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
        component.DisconnectFromParent();
        component.DisconnectAllChildren();
        component.SetSelected(true);
        tempConnectableComponentIDs = GetConnectableGameObject();
        SetAvailableForConnection(tempConnectableComponentIDs, true);
        OnGameComponentDraggedStart?.Invoke(component);


    }

    private async void HandleComponentDraggedEnd(ulong draggableID)
    {
        if(IsOwner){
            RemoveOwnership_ServerRpc(draggableID);
            await UniTask.DelayFrame(2);
            HandleComponentDraggedEndServerRpc(draggableID);
        }
    }
    [ServerRpc]
    private void HandleComponentDraggedEndServerRpc(ulong draggableID){
        

        var component = Utils.GetLocalGameObjectByNetworkID(draggableID)?.GetComponent<IGameComponent>();
        component.SetSelected(false);
        var (availableParent, connectorInfo) = component.GetAvailableConnection();
        if (availableParent != null){
            component.ConnectToParent(availableParent, connectorInfo);
            AfterGameComponentConnected?.Invoke(component);
        }
        SetAvailableForConnection(tempConnectableComponentIDs, false);
        tempConnectableComponentIDs = null;
        OnGameComponentDraggedEnd?.Invoke(component);
    }
    private void SetAvailableForConnection(ICollection<ulong> componentIDs, bool available){
        foreach (var componentID in componentIDs)
        {
            var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
            Debug.Assert(component != null, "component is null");
            component.SetAvailableForConnectionClientRpc(available);
        }
    }
    [ServerRpc]
    private void ChangeOwnership_ServerRpc(ulong componentID)
    {
        var component = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<NetworkObject>();
        component.ChangeOwnership(OwnerClientId);
    }
    [ServerRpc]
    private void RemoveOwnership_ServerRpc(ulong componentID)
    {
        var component = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<NetworkObject>();
        component.RemoveOwnership();
    }

}
