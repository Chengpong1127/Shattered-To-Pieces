using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class Connector : MonoBehaviour, IConnector
{

    public IList<IConnector> ChildConnectors { get; set; }

    public IConnector ParentConnector { get; set; }
    public IGameComponent GameComponent { get; private set; }
    public Rigidbody2D selfRigidbody => GameComponent.BodyRigidbody;
    private Collider2D selfCollider => GameComponent.BodyCollider;
    UnityEvent<bool> attachHandler = new UnityEvent<bool>();

    [SerializeField] AnchoredJoint2D selfJoint;
    [Tooltip("The anchor of the connection point to attach to targets, should be a transform. If null, use the center of the connector as the anchor.")]
    [SerializeField] Transform ConnectionAnchor;
    [SerializeField] List<Target> targetList;

    Target linkedTarget = null;
    Target detectedTarget = null;

    // target detect process
    static ContactFilter2D targetLayerFilter = new ContactFilter2D();
    List<Collider2D> collisionResult = new List<Collider2D>();
    float selectedObjDist;
    float compareObjDist;

    //==================================functions==================================//


    private void Awake() {
        Debug.Assert(selfJoint);
        selfJoint.autoConfigureConnectedAnchor = false;
        linkedTarget = null;
        selectedObjDist = float.PositiveInfinity;

        // initialize for filter
        targetLayerFilter.useLayerMask = true;
        targetLayerFilter.useTriggers = true;
        targetLayerFilter.SetLayerMask(LayerMask.GetMask("targetLayer"));// this string should be Target's layer

        int tid = 0;
        targetList.ForEach(target => {
            target.ownerConnector = this;
            target.targetID = tid++;
        });

        GameComponent = GetComponentInParent<IGameComponent>();
        ChildConnectors = new List<IConnector>();

        if (ConnectionAnchor != null) selfJoint.anchor = (Vector2)ConnectionAnchor.transform.localPosition;
    }

    public void SetConnectMode(bool draggingMode){
        SwitchCombine(draggingMode);
    }
    public void SetSelectingMode(bool selectingMode){
        SwitchSelecting(selectingMode);
    }

    // State chang function
    void SwitchCombine(bool b) {
        if (!b) { SwitchSelecting(false); }
        //selfRigidbody.freezeRotation = b;
        SwitchTargetActive(b);


    }
    public void SwitchSelecting(bool b) {

        if (b) { this.UnlinkToConnector(); }

        //selfRigidbody.gravityScale = b ? 0 : 1;
        SwitchTargetActive(!b);
        attachHandler.Invoke(b);

    }
    void SwitchAttach(bool b) {

        SwitchTargetActive(!b);
        //selfRigidbody.gravityScale = b ? 0 : 1;
        attachHandler.Invoke(b);

    }
    void SwitchTargetActive(bool b) {
        targetList.ForEach(target => {
            target.SwitchActive(b);
        });
    }

    // Target interact
    void LinkToConnector(Connector connector, ConnectionInfo info) {
        if (connector == null ||
            !(connector.targetList.Count > info.linkedTargetID) ||
            !connector.targetList[(int)info.linkedTargetID].LinkToTarget(this)||
            connector.selfJoint.connectedBody == GameComponent.BodyRigidbody
            ) { return; }


        linkedTarget = connector.targetList[(int)info.linkedTargetID];

        linkedTarget.ownerConnector.attachHandler.AddListener(this.SwitchAttach);

        this.selfJoint.connectedBody = connector.selfRigidbody;

        this.selfJoint.connectedAnchor = (Vector2)linkedTarget.targetPoint.transform.localPosition;
        if (ConnectionAnchor != null)
        {
            Vector3 positionOffset = linkedTarget.targetPoint.transform.position - ConnectionAnchor.position;
            transform.parent.position += positionOffset;

        }
        this.selfJoint.enabled = true;
    }
    public void UnlinkToConnector() {
        this.selfJoint.connectedBody = null;
        this.selfJoint.enabled = false;

        if (linkedTarget == null) { return; }

        linkedTarget.UnLinkToTarget();
        linkedTarget.ownerConnector.attachHandler.RemoveListener(this.SwitchAttach);
        linkedTarget = null;
    }
    public void DetectTarget()
    {
        GameObject selectedTargetObj = null;
        selectedObjDist = float.PositiveInfinity;
        selfCollider.OverlapCollider(targetLayerFilter, collisionResult);

        collisionResult.ForEach(c =>
        {
            compareObjDist = selfCollider.Distance(c).distance;
            if (compareObjDist < selectedObjDist&&c.gameObject!=gameObject&&!c.gameObject.transform.IsChildOf(transform))
            {
                selectedTargetObj = c.gameObject;
            }
        });

        detectedTarget = selectedTargetObj?.GetComponent<Target>();
    }
    

    // TestProgram functitons
    public void AssignTargetList(List<Target> tl) {
        targetList = tl;
        int tid = 0;
        targetList.ForEach(t => {
            t.targetID = tid++;
            t.SetOwner(this);
        });
        return;
    }
    public List<Target> GetTargetList() {  return targetList; }

    public void AssignDetectedTarget(Target t) {
        detectedTarget = t;
    }

    public void AssignLinkedTarget(Target t) {
        linkedTarget = t;
    }


    // implement interface

    public GameObject GetTargetObjByIndex(int targetID) {
        return (targetID >= 0 && this.targetList.Count > targetID) ? this.targetList[targetID].gameObject : null;
    }
    public void ConnectToComponent(IConnector connectorPoint, ConnectionInfo info) {
        detectedTarget = connectorPoint?.GetTargetObjByIndex((int)info.linkedTargetID)?.gameObject.GetComponent<Target>();
        if (detectedTarget == null) { return; }
        this.LinkToConnector(detectedTarget.ownerConnector, info);
    }
    public (IConnector, int) GetAvailableConnector() {
        DetectTarget();
        IConnector resIC = null;
        int resTid = -1;

        if (detectedTarget != null)
        {
            resIC = detectedTarget.ownerConnector;
            resTid = detectedTarget.targetID;
            if (resIC == this)
            {
                resIC = null;
                resTid = -1;
            }
        }

        return (resIC, resTid);
    }
    public IInfo Dump() {
        if (linkedTarget == null) {
            return ConnectionInfo.NoConnection();
        }
        var res = new ConnectionInfo();
        res.linkedTargetID = linkedTarget.targetID;
        return res;
    }

    public void Disconnect()
    {
        UnlinkToConnector();
        if(ParentConnector!=null)ParentConnector.ChildConnectors.Remove(this as IConnector);
        ParentConnector = null;
    }

    IList<IConnector> IConnector.GetChildConnectors()
    {
        return ChildConnectors;
    }


    IConnector IConnector.GetParentConnector()
    {
        return ParentConnector;
    }

    GameObject IConnector.GetTargetObjByIndex(int targetID)
    {
        if (targetID < 0 || targetID >= transform.childCount)
        {
            return null; 
        }

        Transform childTransform = transform.GetChild(targetID);
        if (childTransform == null)
        {
            return null; 
        }

        return childTransform.gameObject;
    }

    void IConnector.ConnectToComponent(IConnector connectorPoint, ConnectionInfo info)
    {
        detectedTarget = connectorPoint?.GetTargetObjByIndex((int)info.linkedTargetID)?.gameObject.GetComponent<Target>();
        if (detectedTarget == null) {  return; }
        this.LinkToConnector(detectedTarget.ownerConnector, info);
        this.ParentConnector = connectorPoint;
        connectorPoint.ChildConnectors.Add(this as IConnector);
    }
}
