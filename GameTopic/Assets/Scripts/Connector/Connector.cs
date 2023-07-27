using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem;

public enum ConnectorState {
    INITIAL,
    COMBINE,
    SELECT,
    ATTACH
}

public class Connector : MonoBehaviour, IConnector
{
    public ConnectorState currState { get; set; } = ConnectorState.INITIAL;

    public IList<IConnector> ChildConnectors { get; set; }

    public IConnector ParentConnector { get; set; }
    public IGameComponent GameComponent { get; private set; }

    InputManager inputManager;
    UnityEvent<bool> attachHandler = new UnityEvent<bool>();

    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] Collider2D selfCollider;
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
        Debug.Assert(selfRigidbody);
        Debug.Assert(selfCollider);
        Debug.Assert(selfJoint);
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


    // use for demo
    private void Update() {
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
        currState = ConnectorState.COMBINE;
        selfRigidbody.freezeRotation = b;
        SwitchTargetActive(b);

        if (!b) { currState = ConnectorState.INITIAL; }

        LayerMask componentLayer = 6;
        if(b){
            Physics2D.IgnoreLayerCollision(componentLayer, componentLayer);
        }else{
            Physics2D.IgnoreLayerCollision(componentLayer, componentLayer, false);
        }
    }
    public void SwitchSelecting(bool b) {
        if (currState != ConnectorState.SELECT && currState != ConnectorState.COMBINE) { return; }
        currState = ConnectorState.SELECT;

        if (b) { this.UnlinkToConnector(); }

        selfRigidbody.gravityScale = b ? 0 : 1;
        selfCollider.isTrigger = b;
        SwitchTargetActive(!b);
        attachHandler.Invoke(b);

        if (!b) { currState = ConnectorState.COMBINE; }
    }
    void SwitchAttach(bool b) {
        if (currState != ConnectorState.ATTACH && currState != ConnectorState.COMBINE) { return; }
        currState = ConnectorState.ATTACH;

        SwitchTargetActive(!b);
        selfCollider.isTrigger = b;
        selfRigidbody.gravityScale = b ? 0 : 1;
        attachHandler.Invoke(b);

        if (!b) { currState = ConnectorState.COMBINE; }
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
            connector.selfJoint.connectedBody == this.selfRigidbody
            ) { return; }

        this.transform.rotation = Quaternion.Euler(0, 0, info.connectorRotation);

        linkedTarget = connector.targetList[(int)info.linkedTargetID];

        linkedTarget.ownerConnector.attachHandler.AddListener(this.SwitchAttach);

        this.selfJoint.connectedBody = connector.selfRigidbody;

        this.selfJoint.connectedAnchor = (Vector2)linkedTarget.targetPoint.transform.localPosition;
        if (ConnectionAnchor != null)
        {
            Debug.Log(1);
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





    // connector cintrol
    public void TrackPositionUpdate(Vector2 pos) {
        selfRigidbody.MovePosition(pos);
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
        res.connectorRotation = this.gameObject.transform.rotation.eulerAngles.z;
        return res;
    }
    public void Load(IInfo info)
    {
        Debug.Assert(info is ConnectionInfo);
        var res = info as ConnectionInfo;
        
        
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

    public void SetZRotation(float rotation)
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;

        currentRotation.z = rotation;

        Quaternion newRotation = Quaternion.Euler(currentRotation);

        GameComponent.DragableTransform.rotation = newRotation;
    }

    public void AddZRotation(float rotation){
        Vector3 currentRotation = transform.rotation.eulerAngles;

        currentRotation.z += rotation;

        Quaternion newRotation = Quaternion.Euler(currentRotation);

        GameComponent.DragableTransform.rotation = newRotation;
    }

}
