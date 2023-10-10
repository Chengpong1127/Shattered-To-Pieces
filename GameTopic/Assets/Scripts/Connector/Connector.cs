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
    private Collider2D SelfCollider => GameComponent.BodyColliders.First();

    [Tooltip("The anchor of the connection point to attach to targets, should be a transform. If null, use the center of the connector as the anchor.")]
    List<Target> targetList;

    Target _currentLinkedTarget = null;

    // target detect process
    static ContactFilter2D targetLayerFilter = new();
    private void Awake() {
        Joint = GetComponent<AnchoredJoint2D>();
        targetList = GetComponentsInChildren<Target>().ToList();
        GameComponent = GetComponentInParent<IGameComponent>();

        targetLayerFilter.useLayerMask = true;
        targetLayerFilter.useTriggers = true;
        targetLayerFilter.SetLayerMask(LayerMask.GetMask("targetLayer"));// this string should be Target's layer
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
    private Target FindClosestOverlapTarget()
    {
        List<Collider2D> collisionResult = new();
        if (SelfCollider.OverlapCollider(targetLayerFilter, collisionResult) != 0){
            var targets = collisionResult
                .Select(c => c.GetComponent<Target>())
                .Where(t => t != null && !t.IsConnected && t.OwnerConnector != this)
                .ToList();

            if (targets.Count == 0) return null;

            var closestTarget = targets
                .OrderBy(t => SelfCollider.Distance(t.BodyCollider).distance)
                .First();

            return closestTarget;
        }
        else{
            return null;
        }

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
    public (IConnector, int) GetAvailableConnector() {
        var detectedTarget = FindClosestOverlapTarget();

        if (detectedTarget != null)
        {
            IConnector resIC = detectedTarget.OwnerConnector;
            int resTid = detectedTarget.TargetID;
            if (resIC.Equals(this)) { return (null, -1); }
            return (resIC, resTid);
        }else{
            return (null, -1);
        }
    }
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
        //GameComponent.BodyTransform.SetParent(null);
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
        _currentLinkedTarget.LinkedBy(this);
        Joint.enabled = true;
    }
    void OnJointBreak2D(Joint2D brokenJoint)
    {
        Debug.Log("A joint has just been broken!, joint: " + brokenJoint);
        GameComponent.DisconnectFromParent();
    }
}
