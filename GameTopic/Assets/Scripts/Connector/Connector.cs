using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

/*
 * �Ȯɴ��դ�k : 
 * �� Z �i�J�s��Ҧ��AX �h�X�s��Ҧ�
 * �b�s��Ҧ����ϥηƹ��ӿ������ë����ƹ��즲�ӱN����s�����L����W
 * 
 * �b unity �s�边���ݭn���N rigidbody ��unity ����s���ܥ��}���_�h�|����
 * 
 */
public class Connector : MonoBehaviour, IConnector
{
    public int connectorID { get; set; }    // self unique id

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

    public event Action<IConnector> OnConnectConnector;
    public event Action<int> OnDisconnectConnector;

    private void Awake()
    {
        if(selfRigidbody == null || selfCollider == null || selfJoint == null) { Debug.Log("Missing variable in Connector.\n"); }
        Debug.Assert(selfRigidbody);
        Debug.Assert(selfCollider);
        Debug.Assert(selfJoint);

        // initialize for filter
        targetLayerFilter.useLayerMask = true;
        targetLayerFilter.useTriggers = true;
        targetLayerFilter.SetLayerMask(LayerMask.GetMask("targetLayer"));// this string should be Target's layer


        int loopindex = 0;
        targetList.ForEach(target => {
            target.SetOwner(this);
            target.targetID = loopindex++;
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
        SwitchSelecting(true);
    }
    private void OnMouseUp()
    {
        SwitchSelecting(false);
    }
    private void OnMouseDrag()
    {
        if (!combineMode || !selecting) { return; }
        HitTriggerUpdate();
        TrackPositionUpdate((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // dump connecotr info with ConnectorInfo structure
    public ConnectorInfo Dump()
    {
        ConnectorInfo info = new ConnectorInfo();
        info.connectorID = connectorID;
        info.linkedConnectorID = linkedTarget != null ? linkedTarget.ownerConnector.connectorID : -1;
        info.linkedTargetID = linkedTarget != null ? linkedTarget.targetID : -1;

        return info;
    }

    public void LoadID(int Cid)
    {
        connectorID = Cid;
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

        if(linkedTarget == null) { return; }

        linkedTarget.UnLinkToTarget();
        linkedTarget.ownerConnector.linkedHandler.RemoveListener(this.SwitchLinkingSelect);

        OnDisconnectConnector.Invoke(connectorID);
    }

    // linke connector c to other connector which is record(gameobject) by c itself.
    static void LinkObject(Connector c)
    {
        if(c == null) return;
        if(c.selectedTargetObj == null) return;
        c.linkedTarget = c.selectedTargetObj.GetComponent<Target>();

        Debug.Assert(c.linkedTarget);

        if(c.linkedTarget == null) { return; }

        LinkTarget(c);
    }

    // linke connector c to other connector which is record(Target) by c itself.
    static void LinkTarget(Connector c)
    {
        Debug.Assert(c.linkedTarget);
        Debug.Assert(c.selfJoint);
        Debug.Assert(c.linkedTarget.targetPoint);



        if (c.linkedTarget == null) { return; }

        if(!c.linkedTarget.LinkToTarget(c)) { return; }
        c.linkedTarget.ownerConnector.linkedHandler.AddListener(c.SwitchLinkingSelect);
        c.selfJoint.connectedBody = c.linkedTarget.ownerConnector.selfRigidbody;
        c.selfJoint.connectedAnchor = (Vector2)c.linkedTarget.targetPoint.transform.localPosition;
        c.selfJoint.enabled = true;

        c.OnConnectConnector.Invoke(c.linkedTarget.ownerConnector);
    }

    // control the connector is selected or not.
    void SwitchSelecting(bool b)
    {
        if (!combineMode) { return; }
        if (selecting == b ) return;

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

    // interface imp.

    public GameObject GetTargetObjByIndex(int targetID)
    {
        return targetList.Count > targetID ? targetList[targetID].gameObject : null;
    }

    // implement connect connector to oth connector by interface.

    // not implement rotation yet
    public void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info)
    {
        selectedTargetObj = connectorPoint.GetTargetObjByIndex(info.linkedTargetID);
        Debug.Assert(selectedTargetObj);
        LinkObject(this);
    }
}
