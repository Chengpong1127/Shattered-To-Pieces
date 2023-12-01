using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using Cysharp.Threading.Tasks;
[RequireComponent(typeof(AnchoredJoint2D))]
public class Connector : NetworkBehaviour, IConnector
{
    public event Action OnJointBreak;
    [SerializeField]
    public AnchoredJoint2D Joint;
    public GameComponent GameComponent { get; private set; }
    List<Target> targetList;

    Target _currentLinkedTarget = null;
    private void Awake() {
        Debug.Assert(Joint != null);
        targetList = GetComponentsInChildren<Target>().ToList();
        GameComponent = GetComponentInParent<GameComponent>();
        SetTargetList(targetList);
    }

    public void SetNonConnectedTargetsDisplay(bool b) {
        targetList.ForEach(target => {
            if (!target.IsConnected){
                target.SetTargetDisplay(b);
            }
        });
    }
    public void SetAllTargetDisplay(bool b) {
        targetList.ForEach(target => {
            target.SetTargetDisplay(b);
        });
    }
    
    public void SetTargetList(List<Target> tl) {
        targetList = tl;
        int tid = 0;
        targetList.ForEach(t => {
            t.TargetID = tid++;
            t.SetOwner(this);
        });
    }
    public Target GetTarget(int targetID) => targetList[targetID];
    public IInfo Dump() {
        if (_currentLinkedTarget == null) {
            return ConnectionInfo.NoConnection();
        }
        var res = new ConnectionInfo
        {
            linkedTargetID = _currentLinkedTarget.TargetID
        };
        return res;
    }

    public void Disconnect()
    {
        Disconnect_ClientRpc();
        if (_currentLinkedTarget != null){
            _currentLinkedTarget.Unlink();
            _currentLinkedTarget = null;
        }
        Joint.connectedBody = null;
        Joint.enabled = false;
    }
    [ClientRpc]
    private void Disconnect_ClientRpc(){
        if (!IsServer){
            if (_currentLinkedTarget != null){
                _currentLinkedTarget.Unlink();
                _currentLinkedTarget = null;
            }
            Joint.connectedBody = null;
            Joint.enabled = false;
        }
    }

    public void ConnectToComponent(IConnector newParent, ConnectionInfo info)
    {
        ConnectToComponent_ClientRpc(newParent.GameComponent.NetworkObjectId, info);
        ConnectToComponentInternal(newParent, info);
    }
    [ClientRpc]
    private void ConnectToComponent_ClientRpc(ulong parentID, ConnectionInfo info){
        ConnectToComponent(parentID, info);
    }
    private void ConnectToComponent(ulong parentID, ConnectionInfo info){
        if (!IsServer){
            var newParent = Utils.GetLocalGameObjectByNetworkID(parentID).GetComponent<Connector>();
            ConnectToComponentInternal(newParent, info);
        }
    }
    private void ConnectToComponentInternal(IConnector parentConnector, ConnectionInfo info){
        if (parentConnector == null) throw new ArgumentException("connector is null");
        if (info == null) throw new ArgumentException("info is null");
        _currentLinkedTarget = parentConnector.GetTarget(info.linkedTargetID);
        GameComponent.BodyTransform.position = _currentLinkedTarget.transform.position;
        JointConnection(_currentLinkedTarget.ConnectionPosition, parentConnector.GameComponent.BodyRigidbody);
        _currentLinkedTarget.SetLink(this);
    }
    private void JointConnection(Vector3 ConnectionPosition, Rigidbody2D connectedBody){
        Joint.connectedAnchor = ConnectionPosition;
        Joint.connectedBody = connectedBody;
        Joint.enabled = true;
    }

    public void BreakConnection(){
        BreakConnectionClientRpc();
    }
    [ClientRpc]
    void BreakConnectionClientRpc() {
        OnJointBreak?.Invoke();
    }

    void OnJointBreak2D(Joint2D brokenJoint)
    {
        OnJointBreak?.Invoke();
    }
}
