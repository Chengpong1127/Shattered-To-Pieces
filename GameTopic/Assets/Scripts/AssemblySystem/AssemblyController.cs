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
    private ConnectionInfo tempSelectedConnectionInfo { get; set; }

    /// <summary>
    /// This event will be invoked when a game component is started to drag.
    /// </summary>
    public event Action<IGameComponent> OnGameComponentSelected;
    /// <summary>
    /// This event will be invoked after a game component is dragged and released.
    /// </summary>
    public event Action<IGameComponent> OnGameComponentSelectedEnd;
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
            selectAction?.Disable();
            disconnectAction?.Disable();
            flipAction?.Disable();
            rotateAction?.Disable();
            try{
                CancelLastSelection_ServerRpc();
            }
            catch{
            }
        }
    }
    [ServerRpc]
    private void CancelLastSelection_ServerRpc(){
        _ = CancelLastSelection();
    }
    #region Select
    private void SelectHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var targets = Utils.GetGameObjectsUnderMouse<Target>();
            var components = Utils.GetGameObjectsUnderMouse<GameComponent>();
            if (targets.Length > 0){
                Vector3 mouseWorldPosition = Utils.GetMouseWorldPosition();
                targets = targets.OrderBy(target => Vector3.Distance(target.transform.position, mouseWorldPosition)).ToArray();
                ConnectionInfo connectionInfo = new ConnectionInfo
                {
                    linkedTargetID = targets.First().TargetID,
                };
                TryConnection_ServerRpc(targets.First().OwnerConnector.GameComponent.NetworkObjectId, connectionInfo);
                return;
            }
            else if (components.Length > 0){
                SelectComponentHandler_ServerRpc(components
                    .Where(component => component.CanSelected)
                    .Select(c => c.NetworkObjectId).ToArray());
            }
            else{
                CancelLastSelection_ServerRpc();
            }
        }
    }
    [ServerRpc]
    private void SelectComponentHandler_ServerRpc(ulong[] componentIDs){
        
        componentIDs = componentIDs.Where(id => GetSelectableGameObject().Contains(id)).ToArray();
        if (componentIDs.Length > 0){
            if (componentIDs.First() != SelectedComponentID){
                SelectComponentHandler(componentIDs.First());
            }
        }
    }
    private async void SelectComponentHandler(ulong componentID){
        var gameComponent = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<GameComponent>();
        await CancelLastSelection();
        tempSelectedParentComponentID = gameComponent.Parent == null ? null : (gameComponent.Parent as GameComponent).NetworkObjectId;
        tempSelectedConnectionInfo = gameComponent.Parent == null ? null : (gameComponent.Connector.Dump() as ConnectionInfo);
        gameComponent.Connector.Disconnect();
        gameComponent.DisconnectAllChildren();
        SelectedComponentID = gameComponent.NetworkObjectId;
        
        tempConnectableComponentIDs = GetConnectableGameObject().Where(id => id != SelectedComponentID).ToArray();
        SetAvailableForConnection(tempConnectableComponentIDs, true);
        
        gameComponent.NetworkObject.ChangeOwnership(OwnerClientId);
        await UniTask.NextFrame();
        gameComponent.SetSelected(true);

        OnGameComponentSelected?.Invoke(gameComponent);
    }
    private async UniTask CancelLastSelection(){
        if (SelectedComponentID.HasValue){
            var component = Utils.GetLocalGameObjectByNetworkID(SelectedComponentID.Value)?.GetComponent<GameComponent>();
            component.SetSelected(false);
            OnGameComponentSelectedEnd?.Invoke(component);
            if (tempSelectedParentComponentID.HasValue){
                TryConnection_ServerRpc(tempSelectedParentComponentID.Value, tempSelectedConnectionInfo);
                tempSelectedParentComponentID = null;
                tempSelectedConnectionInfo = null;
            }else{
                component.SetSelected(false);
                OnGameComponentSelectedEnd?.Invoke(component);
                component.NetworkObject.RemoveOwnership();
                SelectedComponentID = null;
            }
            if (tempConnectableComponentIDs != null) SetAvailableForConnection(tempConnectableComponentIDs, false);
            tempConnectableComponentIDs = null;
            await UniTask.WaitUntil(() => !SelectedComponentID.HasValue);
            await UniTask.NextFrame();
        }
    }
    [ServerRpc]
    private void TryConnection_ServerRpc(ulong parentComponentID, ConnectionInfo info){
        if (SelectedComponentID.HasValue && parentComponentID != SelectedComponentID.Value){
            TryConnection(parentComponentID, info);
        }
    }
    private void TryConnection(ulong parentComponentID, ConnectionInfo connectionInfo){
        var component = Utils.GetLocalGameObjectByNetworkID(SelectedComponentID.Value)?.GetComponent<GameComponent>();
        var parentComponent = Utils.GetLocalGameObjectByNetworkID(parentComponentID)?.GetComponent<IGameComponent>();
        if (component.Parent == parentComponent && connectionInfo.linkedTargetID == tempSelectedConnectionInfo.linkedTargetID){
            component.Connector.Disconnect();
            component.Connector.ConnectToComponent(parentComponent.Connector, connectionInfo);
        }else{
            component.DisconnectFromParent();
            component.ConnectToParent(parentComponent, connectionInfo);
        }
        component.SetSelected(false);
        SetAvailableForConnection(tempConnectableComponentIDs, false);
        OnGameComponentSelectedEnd?.Invoke(component);
        tempConnectableComponentIDs = null;
        SelectedComponentID = null;

        AfterGameComponentConnected?.Invoke(component);
    }


    private void DisconnectHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var components = Utils.GetGameObjectsUnderMouse<IGameComponent>();
            if (components.Count() > 0){
                DisconnectServerRpc(components.Select(c => c.NetworkObjectId).ToArray());
            }
        }
    }
    [ServerRpc]
    private void DisconnectServerRpc(ulong[] componentIDs){
        componentIDs = componentIDs.Where(id => GetSelectableGameObject().Contains(id)).ToArray();
        if (componentIDs.Length > 0){
            Disconnect(componentIDs.First());
        }
    }
    private async void Disconnect(ulong componentID){
        await CancelLastSelection();
        var component = Utils.GetLocalGameObjectByNetworkID(componentID).GetComponent<GameComponent>();
        component.DisconnectFromParent();
        component.DisconnectAllChildren();
        component.NetworkObject.RemoveOwnership();
    }
    #endregion
    #region Flip
    private void FlipHandler(InputAction.CallbackContext context){
        if (IsOwner){
            Flip_ServerRpc();
        }

    }
    [ServerRpc]
    private void Flip_ServerRpc(){
        if (SelectedComponentID.HasValue){
            FlipClientRpc(SelectedComponentID.Value);
        }
        
    }
    [ClientRpc]
    private void FlipClientRpc(ulong componentID){
        if (IsOwner){
            var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IAssemblyable>();
            component.AssemblyTransform.localScale = new Vector3(-component.AssemblyTransform.localScale.x, component.AssemblyTransform.localScale.y, component.AssemblyTransform.localScale.z);
        }
    }
    #endregion
    #region Rotate
    private void RotateHandler(InputAction.CallbackContext context){
        if (IsOwner){
            AddRotation_ServerRpc(context.ReadValue<float>());
        }
    }
    [ServerRpc]
    private void AddRotation_ServerRpc(float rotation){
        if(SelectedComponentID.HasValue){
            AddRotation_ClientRpc(SelectedComponentID.Value, rotation * RotationUnit);
        }
        
    }
    [ClientRpc]
    private void AddRotation_ClientRpc(ulong componentID, float rotation){
        if (IsOwner){
            var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IAssemblyable>();
            component.AssemblyTransform.Rotate(new Vector3(0, 0, rotation));
        }
    }
    #endregion
    
    private void SetAvailableForConnection(ICollection<ulong> componentIDs, bool available){
        foreach (var componentID in componentIDs)
        {
            var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<GameComponent>();
            Debug.Assert(component != null, "component is null");
            component.SetAvailableForConnection_ClientRpc(available, OwnerClientId);
        }
    }
}
