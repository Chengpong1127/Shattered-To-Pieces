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


    //==================================functions==================================//


    private void Awake()
    {
        
    }

    void SwitchInitial(bool b) { }
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

    // implement interface

    public GameObject GetTargetObjByIndex(int targetID) { return gameObject; }
    public void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info) { }
    public (IConnector, int) GetAvailableConnector() { return (this, 0); }
    public ConnectorInfo Dump() {
        var res = new ConnectorInfo();
        return res;
    }
}
