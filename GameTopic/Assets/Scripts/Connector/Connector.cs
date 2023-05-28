using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;


public struct ConnectorInfo
{
    public int connectorID;
    public int linkedConnectorID;
    public int linkedTargetID;
    public List<int> ownTargetID;
}


public class Connector : MonoBehaviour, IConnector
{
    int connectorID;    // self unique id

    bool combineMode;   // is the game in combine mode
    bool selecting;     // is this connector be selecting


    // Some variable Connector should have and should be assign in unity editor.
    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] Collider2D selfCollider;
    [SerializeField] AnchoredJoint2D selfJoint;
    [SerializeField] List<Target> targetList;

    // for record connector linking state
    public UnityEvent<bool> linkedHandler;
    GameObject selectedTargetObj;
    Target linkedTarget;

    // some variable for detect hited trigget when the connector itself be selecting
    static ContactFilter2D targetLayerFilter = new ContactFilter2D();
    float selectedObjDist;
    float compareObjDist;
    List<Collider2D> collisionResult;
    Vector2 movePosition;

    private void Start()
    {
        if(selfRigidbody == null || selfCollider == null || selfJoint == null) { Debug.Log("Missing variable in Connector.\n"); }

        // initialize for filter
        targetLayerFilter.useLayerMask = true;
        targetLayerFilter.useTriggers = true;
        targetLayerFilter.SetLayerMask(LayerMask.GetMask("targetLayer"));// this string should be Target's layer



        targetList.ForEach(target => {
            target.SetOwner(this);
        });
        selectedObjDist = float.PositiveInfinity;
        selectedTargetObj = null;
        linkedTarget = null;
        collisionResult = new List<Collider2D>();
        linkedHandler = new UnityEvent<bool>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            SwitchCombineMode(true);
        }
        if (Input.GetKey(KeyCode.X))
        {
            SwitchCombineMode(false);
        }
    }

    private void OnMouseDown()
    {
        if(!combineMode) { return; }
        SwitchSelecting(true);
    }
    private void OnMouseUp()
    {
        if (!combineMode) { return; }
        SwitchSelecting(false);
    }
    private void OnMouseDrag()
    {
        if (!combineMode || !selecting) { return; }
        HitTriggerUpdate();
        TrackPositionUpdate((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // dump connecotr info with ConnectorInfo structure
    ConnectorInfo Dump()
    {
        ConnectorInfo info = new ConnectorInfo();
        info.ownTargetID = new List<int>();
        info.connectorID = connectorID;
        info.linkedConnectorID = linkedTarget != null ? linkedTarget.ownerConnector.connectorID : -1;
        info.linkedTargetID = linkedTarget != null ? linkedTarget.targetID : -1;
        targetList.ForEach(target =>
        {
            info.ownTargetID.Add(target.targetID);
        });
        return info;
    }

    void LoadID(int Cid)
    {
        connectorID = Cid;

        int index = 0;
        targetList.ForEach(target =>
        {
            target.targetID = index++;
        });
    }

    void LoadLink(Target lt)
    {
        linkedTarget = lt;
        LinkTarget(this);
    }

    void LoadLink(Connector oc, int tid)
    {
        linkedTarget = oc.targetList.Count >= tid ? oc.targetList[tid] : null ;
        LinkTarget(this);
    }

    // move connector to  position by rigidbody.
    void TrackPositionUpdate(Vector2 pos)
    {
        selfRigidbody.MovePosition(pos);
    }

    // show target in screen or not.
    void SwitchTargetActive(bool b)
    {
        targetList.ForEach(target => {
            target.SwitchActive(b);
        });
    }

    // update collider list to check the connector hit which target.
    // update every frame when connector be select.
    void HitTriggerUpdate()
    {
        selfCollider.OverlapCollider(targetLayerFilter, collisionResult);
        if(collisionResult.Count == 0) {
            selectedTargetObj = null;
            selectedObjDist = float.PositiveInfinity;
            return;
        }
        collisionResult.ForEach(c =>
        {
            compareObjDist = selfCollider.Distance(c).distance;
            if(compareObjDist < selectedObjDist)
            {
                selectedTargetObj = c.gameObject;
            }
        });
    }

    // call this function when the connector link to other connector is selected ot attach that selected connector.
    // process by connector themself.
    public void SwitchLinkingSelect(bool b)
    {
        SwitchTargetActive(!b);
        selfCollider.isTrigger = b;
        selfRigidbody.gravityScale = b ? 0 : 1;
        linkedHandler.Invoke(b);
    }

    // reset when the selecting start.
    void ResetCompareVariable()
    {
        selectedTargetObj = null;
        selectedObjDist = float.PositiveInfinity;
        linkedTarget = null;
    }

    // break the link between two connector.
    void BreakTatgetLink() {
        selfJoint.enabled = false;
        linkedTarget?.UnLinkTarget();
    }

    // linke connector c to other connector which is record(gameobject) by c itself.
    static void LinkObject(Connector c)
    {
        if(c == null) return;
        if(c.selectedTargetObj == null || c.collisionResult.Count == 0) return;
        c.linkedTarget = c.selectedTargetObj.GetComponent<Target>();
        if(c.linkedTarget == null) { return; }

        LinkTarget(c);
    }

    // linke connector c to other connector which is record(Target) by c itself.
    static void LinkTarget(Connector c)
    {
        if (c.linkedTarget == null) { return; }

        c.linkedTarget.LinkTarget(c);
        c.selfJoint.connectedBody = c.linkedTarget.ownerConnector.selfRigidbody;
        c.selfJoint.connectedAnchor = c.selectedTargetObj.transform.localPosition;
        c.selfJoint.enabled = true;
    }

    // control the connector is selected or not.
    void SwitchSelecting(bool b)
    {
        selecting = b;
        selfRigidbody.gravityScale = b ? 0 : 1;
        selfCollider.isTrigger = b;
        BreakTatgetLink();
        if (b) ResetCompareVariable(); else LinkObject(this); 
        SwitchTargetActive(!b);
        linkedHandler.Invoke(b);
    }

    // control the connector to switch to combine-mode or not.
    void SwitchCombineMode(bool b)
    {
        if(!b) SwitchSelecting(false);

        combineMode = b;
        selfRigidbody.freezeRotation = b;
        SwitchTargetActive(b);
    }

    public void ConnectToComponent(IConnector connecterPoint, int targetID)
    {
        throw new System.NotImplementedException();
    }

    public void ConnectToComponent(Connector linkedConnector, int targetID)
    {
        this.LoadLink(linkedConnector, targetID);
    }
}
