using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public enum ConnectorState {
    INITIAL,
    COMBINE,
    SELECT,
    ATTACH
}

public class Connector : MonoBehaviour, IConnector
{
    public ConnectorState currState { get; set; } = ConnectorState.INITIAL;

    IGameComponent IConnector.GameComponent => throw new NotImplementedException();

    UnityEvent<bool> attachHandler = new UnityEvent<bool>();
    Vector2 movePosition;

    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] Collider2D selfCollider;
    [SerializeField] AnchoredJoint2D selfJoint;
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
    }

    // use for demo
    private void Update() {
        if (Input.GetKey(KeyCode.Z)) {
            SwitchCombine(true);
        }
        if (Input.GetKey(KeyCode.X)) {
            SwitchCombine(false);
        }
    }
    public void SetConnectMode(bool connectMode){
        if (connectMode){
            SwitchCombine(true);
        }else{
            SwitchCombine(false);
        }
    }

    // State chang function
    void SwitchCombine(bool b) {
        if (!b) { SwitchSelecting(false); }
        currState = ConnectorState.COMBINE;
        selfRigidbody.freezeRotation = b;
        SwitchTargetActive(b);

        if (!b) { currState = ConnectorState.INITIAL; }
    }
    void SwitchSelecting(bool b) {
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
            !connector.targetList[(int)info.linkedTargetID].LinkToTarget(this)
            ) { return; }

        this.transform.rotation = Quaternion.Euler(0, 0, info.connectorRotation);

        linkedTarget = connector.targetList[(int)info.linkedTargetID];

        linkedTarget.ownerConnector.attachHandler.AddListener(this.SwitchAttach);
        this.selfJoint.connectedBody = connector.selfRigidbody;
        this.selfJoint.connectedAnchor = (Vector2)linkedTarget.targetPoint.transform.localPosition;
        this.selfJoint.enabled = true;
    }
    public void UnlinkToConnector() {
        this.selfJoint.enabled = false;

        if (linkedTarget == null) { return; }

        linkedTarget.UnLinkToTarget();
        linkedTarget.ownerConnector.attachHandler.RemoveListener(this.SwitchAttach);
        linkedTarget = null;
    }
    public void DetectTarget() {
        GameObject selectedTargetObj = null;
        selectedObjDist = float.PositiveInfinity;
        selfCollider.OverlapCollider(targetLayerFilter, collisionResult);

        collisionResult.ForEach(c => {
            compareObjDist = selfCollider.Distance(c).distance;
            if (compareObjDist < selectedObjDist) {
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
        IConnector resIC = detectedTarget == null ? null : detectedTarget.ownerConnector;
        int resTid = detectedTarget == null ? -1 : detectedTarget.targetID;
        return (resIC, resTid);
    }
    public IInfo Dump() {
        var res = new ConnectionInfo();
        res.linkedTargetID = linkedTarget == null ? -1 : linkedTarget.targetID;
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
    }

    IList<IConnector> IConnector.GetChildConnectors()
    {
        List<IConnector> childConnectors = new List<IConnector>();


        Transform[] childTransforms = transform.GetComponentsInChildren<Transform>(true);


        foreach (Transform childTransform in childTransforms)
        {
            IConnector childConnector = childTransform.GetComponent<IConnector>();
            if (childConnector != null)
            {
                childConnectors.Add(childConnector);
            }
        }

        return childConnectors;

    }

    IConnector IConnector.GetParentConnector()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            return null;
        }

        IConnector parentConnector = parentTransform.GetComponentInParent<IConnector>();
        return parentConnector;
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
        if (connectorPoint == null)
        {
            return;
        }

        Connector connector = connectorPoint as Connector;
        if (connector == null)
        {
            return;
        }

        if (connector.targetList == null || connector.targetList.Count <= info.linkedTargetID)
        {
            return;
        }

        Target target = connector.targetList[(int)info.linkedTargetID];
        if (!target.LinkToTarget(this))
        {
            return;
        }

        this.transform.rotation = Quaternion.Euler(0, 0, info.connectorRotation);
        linkedTarget = target;

        linkedTarget.ownerConnector.attachHandler.AddListener(this.SwitchAttach);
        this.selfJoint.connectedBody = connector.selfRigidbody;
        this.selfJoint.connectedAnchor = (Vector2)linkedTarget.targetPoint.transform.localPosition;
        this.selfJoint.enabled = true;
    }

    void IConnector.UnlinkToConnector()
    {
        UnlinkToConnector();
    }

    void IConnector.Disconnect()
    {
        UnlinkToConnector();
    }

    void IConnector.SetConnectMode(bool connectMode)
    {
        if (connectMode)
        {
            SwitchCombine(true);
        }
        else
        {
            SwitchCombine(false);
        }
    }
}
