using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class ConnectorTest
{
    [Test]
    public void ConnectorExistTest() {
        GameObject squ_obj = Resources.Load<GameObject>("Square");
        Connector c = squ_obj?.GetComponentInChildren<Connector>();
        Assert.IsNotNull(c);
    }

    [Test]
    public void Connector_IMP_GetTargetObjByIndex() {
        Connector C_obj = Resources.Load<GameObject>("Square").GetComponentInChildren<Connector>();
        Connector c = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);

        Target T_obj = Resources.Load<GameObject>("T_Obj")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }

        c.connectorID = 0;
        c.AssignTargetList(tl);

        Assert.AreEqual(c.GetTargetObjByIndex(-1), null);
        Assert.AreEqual(c.GetTargetObjByIndex(0), tl[0].gameObject);
        Assert.AreEqual(c.GetTargetObjByIndex(1), tl[1].gameObject);
        Assert.AreEqual(c.GetTargetObjByIndex(2), tl[2].gameObject);
        Assert.AreEqual(c.GetTargetObjByIndex(3), tl[3].gameObject);
        Assert.AreEqual(c.GetTargetObjByIndex(4), null);
    }

    [Test]
    public void Connector_IMP_Dump() {
        Connector C_obj = Resources.Load<GameObject>("Square").GetComponentInChildren<Connector>();

        Connector c = MonoBehaviour.Instantiate(C_obj);
        Connector c2 = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);
        Assert.IsNotNull(c2);

        Target T_obj = Resources.Load<GameObject>("T_Obj")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }


        c.connectorID = 0;
        c.AssignTargetList(tl);

        c2.connectorID = 1;
        c2.AssignLinkedTarget(tl[1]);

        ConnectorInfo info = new ConnectorInfo();
        info.connectorID = 1;
        info.linkedConnectorID = 0;
        info.linkedTargetID = 1;
        info.connectorRotation = 0f;


        Assert.AreEqual(c.Dump(), ConnectorInfo.NoConnection(c.connectorID));
        Assert.AreEqual(c2.Dump(), info);
    }

    [Test]
    public void Connector_IMP_ConnectToComponent() {
        Connector C_obj = Resources.Load<GameObject>("Square").GetComponentInChildren<Connector>();

        Connector c = MonoBehaviour.Instantiate(C_obj);
        Connector c2 = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);
        Assert.IsNotNull(c2);

        Target T_obj = Resources.Load<GameObject>("T_Obj")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }

        c.connectorID = 0;
        c.AssignTargetList(tl);
        ConnectorInfo info = new ConnectorInfo();
        info.connectorID = 1;
        info.linkedConnectorID = 0;
        info.linkedTargetID = 1;
        info.connectorRotation = 0.5f;

        c2.connectorID = 1;
        c2.ConnectToComponent(c, info);

        ConnectorInfo c2info = c2.Dump();
        Debug.Log(c2info.connectorID);
        Debug.Log(c2info.linkedConnectorID);
        Debug.Log(c2info.linkedTargetID);
        Debug.Log(c2info.connectorRotation);

        Assert.AreEqual(c.Dump(), ConnectorInfo.NoConnection(c.connectorID));
        Assert.AreEqual(c2.Dump(), info);
    }
    [Test]
    public void Connector_IMP_GetAvailableConnector() {
        Connector C_obj = Resources.Load<GameObject>("Square").GetComponentInChildren<Connector>();

        Connector c = MonoBehaviour.Instantiate(C_obj);
        Connector c2 = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);
        Assert.IsNotNull(c2);

        Target T_obj = Resources.Load<GameObject>("T_Obj")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }

        c.connectorID = 0;
        c.AssignTargetList(tl);

        c2.connectorID = 1;
        c2.AssignDetectedTarget(tl[2]);

        (IConnector ic, int tid) = c2.GetAvailableConnector();

        Assert.AreEqual(tid, 2);
        Assert.AreEqual(c, ic?.GetTargetObjByIndex(tid)?.GetComponent<Target>()?.ownerConnector);
    }
}
