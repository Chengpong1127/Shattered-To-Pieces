using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ConnectorState
{
    INITIAL,
    COMBINE,
    SELECT,
    ATTACH
}

public class RBconnector : MonoBehaviour, IConnector
{
    public int connectorID { get; set; }
    public ConnectorState currState { get; set; } = ConnectorState.INITIAL;
    UnityEvent<bool> attachHandler = new UnityEvent<bool>();

    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] Collider2D selfCollider;
    [SerializeField] AnchoredJoint2D selfJoint;
    [SerializeField] List<Target> targetList;

    Target linkedTarget = null;


    //==================================functions==================================//


    private void Awake()
    {
    }


    // State chang function
    void SwitchCombine(bool b) {
        if(!b) { SwitchSelecting(false); }
        currState = ConnectorState.COMBINE;
        selfRigidbody.freezeRotation = b;
        SwitchTargetActive(b);

        if(!b) { currState = ConnectorState.INITIAL; }
    }
    void SwitchSelecting(bool b) {
        if(currState != ConnectorState.SELECT && currState != ConnectorState.COMBINE) { return; }
        currState = ConnectorState.SELECT;

        selfRigidbody.gravityScale = b ? 0 : 1;
        selfCollider.isTrigger = b;
        SwitchTargetActive(!b);
        attachHandler.Invoke(b);
    }
    void SwitchAttach(bool b) {
        if (currState != ConnectorState.ATTACH && currState != ConnectorState.COMBINE) { return; }
        currState = ConnectorState.ATTACH;

        SwitchTargetActive(!b);
        selfCollider.isTrigger = b;
        selfRigidbody.gravityScale = b ? 0 : 1;
        attachHandler.Invoke(b);
    }
    void SwitchTargetActive(bool b) {
        targetList.ForEach(target => {
            target.SwitchActive(b);
        });
    }

    // Target interact
    void LinkToConnector(RBconnector connector, ConnectorInfo info) {
        if (connector.connectorID != info.connectorID ||
             this.connectorID != info.linkedConnectorID ||
             !(connector.targetList.Count > info.linkedTargetID) ||
             !connector.targetList[info.linkedTargetID].RB_LinkToTarget(this)
            ) { return; }

        this.transform.rotation = Quaternion.Euler(0, 0, info.connectorRotation);

        connector.targetList[info.linkedTargetID].ownerRBconnector.attachHandler.AddListener(this.SwitchAttach);
        this.selfJoint.connectedBody = connector.selfRigidbody;
        this.selfJoint.connectedAnchor = (Vector2)connector.targetList[info.linkedTargetID].targetPoint.transform.localPosition;
        this.selfJoint.enabled = true;
    }
    void UnlinkToConnector() {
        this.selfJoint.enabled = false;

        if (linkedTarget == null) { return; }
         
        linkedTarget.RB_UnLinkToTarget();
        linkedTarget.ownerRBconnector.attachHandler.RemoveListener(this.SwitchAttach);
        linkedTarget = null;
    }
    Target DetectTarget() {
        return new Target();
    }


    // implement interface

    public GameObject GetTargetObjByIndex(int targetID) { return gameObject; }
    public void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info) { }
    public (IConnector, int) GetAvailableConnector() { return (this, 0); }
    public ConnectorInfo Dump() {
        var res = new ConnectorInfo();
        res.connectorID = this.connectorID;
        res.linkedConnectorID = linkedTarget == null ? -1 : linkedTarget.ownerRBconnector.connectorID;
        res.linkedTargetID = linkedTarget == null ? -1 : linkedTarget.targetID;
        res.connectorRotation = Quaternion.Angle(this.gameObject.transform.rotation, Quaternion.identity);
        return res;
    }
}
