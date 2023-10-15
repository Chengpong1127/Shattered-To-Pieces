using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
[RequireComponent(typeof(AnchoredJoint2D))]
public class Connector : MonoBehaviour, IConnector
{
    public AnchoredJoint2D Joint { get; set; }
    public IGameComponent GameComponent { get; private set; }
    List<Target> targetList;

    Target _currentLinkedTarget = null;
    private void Awake() {
        Joint = GetComponent<AnchoredJoint2D>();
        targetList = GetComponentsInChildren<Target>().ToList();
        GameComponent = GetComponentInParent<IGameComponent>();
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
        if (_currentLinkedTarget != null){
            _currentLinkedTarget.Unlink();
            _currentLinkedTarget = null;
        }
        Joint.connectedBody = null;
        Joint.enabled = false;
    }

    public void ConnectToComponent(IConnector newParent, ConnectionInfo info)
    {
        if (newParent == null) throw new ArgumentException("newParent is null");
        if (info == null) throw new ArgumentException("info is null");
        _currentLinkedTarget = newParent.GetTarget(info.linkedTargetID);
        Joint.connectedAnchor = _currentLinkedTarget.ConnectionPosition;
        Joint.connectedBody = newParent.GameComponent.BodyRigidbody;
        _currentLinkedTarget.SetLink(this);
        Joint.enabled = true;
    }
    void OnJointBreak2D(Joint2D brokenJoint)
    {
        Debug.Log("A joint broken, joint: " + brokenJoint);
        GameComponent.DisconnectFromParent();
    }
}
