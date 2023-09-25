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
            CancelLastSelection_ServerRpc();
        }
    }
    #region Select
    private void SelectHandler(InputAction.CallbackContext context){
        if (IsOwner){
            var targets = Utils.GetGameObjectsUnderMouse<Target>();
            if (targets.Length > 0){
                TryConnection_ServerRpc(targets.First().OwnerConnector.GameComponent.NetworkObjectId, targets.First().TargetID);
                return;
            }
            var components = Utils.GetGameObjectsUnderMouse<IGameComponent>();
            if (components.Length > 0){
                SelectComponentHandler_ServerRpc(components.Select(c => c.NetworkObjectId).ToArray());
            }
        }
    }
    [ServerRpc]
    private void SelectComponentHandler_ServerRpc(ulong[] componentIDs){
        
        componentIDs = componentIDs.Where(id => GetSelectableGameObject().Contains(id)).ToArray();
        if (componentIDs.Length > 0){
            if (componentIDs.First() != SelectedComponentID){
                Debug.Log("SelectComponentHandler_ServerRpc");
                var gameComponent = Utils.GetLocalGameObjectByNetworkID(componentIDs.First()).GetComponent<IGameComponent>();
                CancelLastSelection_ServerRpc();
                tempSelectedParentComponentID = gameComponent.Parent == null ? null : (gameComponent.Parent as IGameComponent).NetworkObjectId;
                tempConnectionInfo = gameComponent.Parent == null ? null : (gameComponent.Connector.Dump() as ConnectionInfo);
                gameComponent.DisconnectFromParent();
                gameComponent.DisconnectAllChildren();
                SelectedComponentID = gameComponent.NetworkObjectId;
                gameComponent.SetSelected(true);
                tempConnectableComponentIDs = GetConnectableGameObject();
                SetAvailableForConnection(tempConnectableComponentIDs, true);
            }
        }
    }
    [ServerRpc]
    private void CancelLastSelection_ServerRpc(){
        if (SelectedComponentID.HasValue){
            Utils.GetLocalGameObjectByNetworkID(SelectedComponentID.Value)?.GetComponent<IGameComponent>().SetSelected(false);
            if (tempSelectedParentComponentID.HasValue){
                TryConnection_ServerRpc(tempSelectedParentComponentID.Value, tempConnectionInfo.linkedTargetID);
                tempSelectedParentComponentID = null;
                tempConnectionInfo = null;
            }
            SelectedComponentID = null;
        }
    }
    [ServerRpc]
    private void TryConnection_ServerRpc(ulong parentComponentID, int targetID){
        if (SelectedComponentID.HasValue){
            var component = Utils.GetLocalGameObjectByNetworkID(SelectedComponentID.Value)?.GetComponent<IGameComponent>();
            var parentComponent = Utils.GetLocalGameObjectByNetworkID(parentComponentID)?.GetComponent<IGameComponent>();
            var connectionInfo = new ConnectionInfo
            {
                linkedTargetID = targetID,
            };
            component.SetSelected(false);
            component.ConnectToParent(parentComponent, connectionInfo);
            SetAvailableForConnection(tempConnectableComponentIDs, false);
            tempConnectableComponentIDs = null;
            SelectedComponentID = null;
        }
        
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
        CancelLastSelection_ServerRpc();
        componentIDs = componentIDs.Where(id => GetSelectableGameObject().Contains(id)).ToArray();
        if (componentIDs.Length > 0){
            var component = Utils.GetLocalGameObjectByNetworkID(componentIDs.First())?.GetComponent<IGameComponent>();
            Debug.Assert(component != null, "component is null");
            component.DisconnectFromParent();
            component.DisconnectAllChildren();
        }
        
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
            var component = Utils.GetLocalGameObjectByNetworkID(SelectedComponentID.Value)?.GetComponent<IAssemblyable>();
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
            var component = Utils.GetLocalGameObjectByNetworkID(SelectedComponentID.Value)?.GetComponent<IAssemblyable>();
            component.AssemblyTransform.Rotate(new Vector3(0, 0, rotation * RotationUnit));
        }
        
    }
    #endregion
    
    private void SetAvailableForConnection(ICollection<ulong> componentIDs, bool available){
        foreach (var componentID in componentIDs)
        {
            var component = Utils.GetLocalGameObjectByNetworkID(componentID)?.GetComponent<IGameComponent>();
            Debug.Assert(component != null, "component is null");
            component.SetAvailableForConnectionClientRpc(available);
        }
    }
}
