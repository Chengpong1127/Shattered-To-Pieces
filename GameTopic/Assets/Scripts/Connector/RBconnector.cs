using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    [SerializeField] Rigidbody2D selfRigidbody;
    [SerializeField] Collider2D selfCollider;
    [SerializeField] AnchoredJoint2D selfJoint;
    [SerializeField] List<Target> targetList;


    //==================================functions==================================//


    private void Awake()
    {
        
    }








    public GameObject GetTargetObjByIndex(int targetID) { return gameObject; }
    public void ConnectToComponent(IConnector connectorPoint, ConnectorInfo info) { }
    public (IConnector, int) GetAvailableConnector() { return (this, 0); }
    public ConnectorInfo Dump() {
        var res = new ConnectorInfo();
        return res;
    }
}
