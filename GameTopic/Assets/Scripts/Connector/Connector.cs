using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class Connector : MonoBehaviour
{
    bool combineMode;   // is the game in combine mode
    bool selecting;     // is this connector be selecting


    // Some variable Connector should have and should be assign in unity editor.
    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] Collider2D selfCollider;
    [SerializeField] AnchoredJoint2D selfJoint;
    [SerializeField] List<Target> targetList;

    public UnityEvent<bool> linkedHandler;
    GameObject selectedTargetObj;
    Target linkedTarget;

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


    List<Target> Dump()
    {
        return targetList;
    }


    void TrackPositionUpdate(Vector2 pos)
    {
        selfRigidbody.MovePosition(pos);
    }
    void SwitchTargetActive(bool b)
    {
        targetList.ForEach(target => {
            target.SwitchActive(b);
        });
    }

    void HitTriggerUpdate()
    {
        selfCollider.OverlapCollider(targetLayerFilter, collisionResult);
        if(collisionResult.Count == 0) { selectedTargetObj = null; return; }
        collisionResult.ForEach(c =>
        {
            compareObjDist = selfCollider.Distance(c).distance;
            if(compareObjDist < selectedObjDist)
            {
                selectedTargetObj = c.gameObject;
            }
        });
    }
    public void SwitchLinkingSelect(bool b)
    {
        SwitchTargetActive(!b);
        selfCollider.isTrigger = b;
        selfRigidbody.gravityScale = b ? 0 : 1;
        linkedHandler.Invoke(b);
    }
    void ResetCompareVariable()
    {
        selectedTargetObj = null;
        selectedObjDist = float.PositiveInfinity;
        linkedTarget = null;
    }
    void BreakTatgetLink() {
        selfJoint.enabled = false;
        linkedTarget?.UnLinkTarget();
    }
    static void LinkTarget(Connector c)
    {
        if(c == null) return;
        if(c.selectedTargetObj == null || c.collisionResult.Count == 0) return;
        c.linkedTarget = c.selectedTargetObj.GetComponent<Target>();
        if(c.linkedTarget == null) { return; }

        c.linkedTarget.LinkTarget(c);
        c.selfJoint.connectedBody = c.linkedTarget.ownerConnector.selfRigidbody;
        c.selfJoint.connectedAnchor = c.selectedTargetObj.transform.localPosition;
        c.selfJoint.enabled = true;
    }
    void SwitchSelecting(bool b)
    {
        selecting = b;
        selfRigidbody.gravityScale = b ? 0 : 1;
        selfCollider.isTrigger = b;
        BreakTatgetLink();
        if (b) ResetCompareVariable(); else LinkTarget(this); 
        SwitchTargetActive(!b);
        linkedHandler.Invoke(b);
    }
    void SwitchCombineMode(bool b)
    {
        combineMode = b;
        selfRigidbody.freezeRotation = b;
        SwitchTargetActive(b);
    }
}
