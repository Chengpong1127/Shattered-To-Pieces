using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
[RequireComponent(typeof(AnchoredJoint2D))]
public class Connector : NetworkBehaviour, IConnector
{
    public event Action OnJointBreak;
    public AnchoredJoint2D Joint { get; set; }
    public GameComponent GameComponent { get; private set; }
    List<Target> targetList;

    Target _currentLinkedTarget = null;
    private void Awake() {
        Joint = GetComponent<AnchoredJoint2D>();
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
        if (newParent == null) throw new ArgumentException("newParent is null");
        if (info == null) throw new ArgumentException("info is null");
        _currentLinkedTarget = newParent.GetTarget(info.linkedTargetID);
        Joint.connectedAnchor = _currentLinkedTarget.ConnectionPosition;
        Joint.connectedBody = newParent.GameComponent.BodyRigidbody;
        _currentLinkedTarget.SetLink(this);
        Joint.enabled = true;
        GameComponent.HaveConnected = true;
        Debug.Log(1);
    }
    [ClientRpc]
    private void ConnectToComponent_ClientRpc(ulong parentID, ConnectionInfo info){
        if (!IsServer){
            var newParent = Utils.GetLocalGameObjectByNetworkID(parentID).GetComponent<IConnector>();
            if (newParent == null) throw new ArgumentException("newParent is null");
            if (info == null) throw new ArgumentException("info is null");
            _currentLinkedTarget = newParent.GetTarget(info.linkedTargetID);
            Joint.connectedAnchor = _currentLinkedTarget.ConnectionPosition;
            Joint.connectedBody = newParent.GameComponent.BodyRigidbody;
            _currentLinkedTarget.SetLink(this);
            Joint.enabled = true;
            GameComponent.HaveConnected=true;
            Debug.Log(1);
        }

    }
    void OnJointBreak2D(Joint2D brokenJoint)
    {
        OnJointBreak?.Invoke();
    }
}
