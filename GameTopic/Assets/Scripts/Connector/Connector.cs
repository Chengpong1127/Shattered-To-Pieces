using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Connector : MonoBehaviour, IConnector
{
    public IGameComponent GameComponent { get; private set; }
    private Collider2D SelfCollider => GameComponent.BodyCollider;

    [Tooltip("The anchor of the connection point to attach to targets, should be a transform. If null, use the center of the connector as the anchor.")]
    [SerializeField] Transform ConnectionAnchor;
    [SerializeField] List<Target> targetList;

    Target _currentLinkedTarget = null;

    // target detect process
    static ContactFilter2D targetLayerFilter = new();
    

    //==================================functions==================================//


    private void Awake() {
        // Debug.Assert(selfJoint);
        // selfJoint.autoConfigureConnectedAnchor = false;

        // initialize for filter

        GameComponent = GetComponentInParent<IGameComponent>();

        targetLayerFilter.useLayerMask = true;
        targetLayerFilter.useTriggers = true;
        targetLayerFilter.SetLayerMask(LayerMask.GetMask("targetLayer"));// this string should be Target's layer
        SetTargetList(targetList);

        

        //if (ConnectionAnchor != null) selfJoint.anchor = (Vector2)ConnectionAnchor.transform.localPosition;
    }
    public void SetAllTargetsDisplay(bool b) {
        targetList.ForEach(target => {
            target.SetTargetDisplay(b);
        });
    }

    public void SetNonConnectedTargetsDisplay(bool b) {
        targetList.ForEach(target => {
            if (!target.IsConnected){
                target.SetTargetDisplay(b);
            }
        });
    }

    private void LinkToConnector(Connector connector, ConnectionInfo info) {

        _currentLinkedTarget = connector.targetList[info.linkedTargetID];
        GameComponent.BodyTransform.SetParent(_currentLinkedTarget.transform);

        //selfJoint.connectedBody = connector.SelfRigidbody;
        //selfJoint.connectedAnchor = (Vector2)_currentLinkedTarget.targetPoint.transform.localPosition;
        if (ConnectionAnchor != null)
        {
            Vector3 positionOffset = _currentLinkedTarget.gameObject.transform.position - ConnectionAnchor.position;
            GameComponent.BodyTransform.position += positionOffset;
        }
        else{
            GameComponent.BodyTransform.localPosition = Vector3.zero;
        }
        //selfJoint.enabled = false;
    }
    private void UnlinkToConnector() {
        //selfJoint.connectedBody = null;
        //selfJoint.enabled = false;
        GameComponent.BodyTransform.SetParent(null);

        if (_currentLinkedTarget == null) { return; }

        _currentLinkedTarget.Unlink();
        _currentLinkedTarget = null;
    }
    private Target FindClosestOverlapTarget()
    {
        
        
        List<Collider2D> collisionResult = new();
        SelfCollider.OverlapCollider(targetLayerFilter, collisionResult);

        GameObject selectedTargetObj = null;
        float distance = float.PositiveInfinity;
        collisionResult.RemoveAll(c => c.gameObject == gameObject || c.gameObject.transform.IsChildOf(transform) || c.GetComponent<Target>() == null);
        collisionResult.ForEach(c =>
        {
            var compareObjDist = SelfCollider.Distance(c).distance;
            if (compareObjDist < distance)
            {
                selectedTargetObj = c.gameObject;
            }
        });

        return selectedTargetObj?.GetComponent<Target>();
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
        GameComponent.BodyRigidbody.isKinematic = false;
        UnlinkToConnector();
    }

    public void ConnectToComponent(IConnector newParent, ConnectionInfo info)
    {
        if (newParent == null) throw new ArgumentException("newParent is null");
        if (info == null) throw new ArgumentException("info is null");
        GameComponent.BodyRigidbody.isKinematic = true;
        var target = newParent.GetTarget(info.linkedTargetID);
        LinkToConnector(target.OwnerConnector, info);
    }
}
