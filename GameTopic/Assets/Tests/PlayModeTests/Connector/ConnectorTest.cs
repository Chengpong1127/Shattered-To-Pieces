using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
public class ConnectorTest
{
    [Test]
    public void ConnectorExistTest() {
        GameObject squ_obj = Resources.Load<GameObject>("Prefabs/Square");
        Connector c = squ_obj?.GetComponentInChildren<Connector>();
        Assert.IsNotNull(c);
    }

    [Test]
    public void Connector_IMP_GetTargetObjByIndex() {
        Connector C_obj = Resources.Load<GameObject>("Prefabs/Square").GetComponentInChildren<Connector>();
        Connector c = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);

        Target T_obj = Resources.Load<GameObject>("Prefabs/Target")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }

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
        Connector C_obj = Resources.Load<GameObject>("Prefabs/Square").GetComponentInChildren<Connector>();

        Connector c = MonoBehaviour.Instantiate(C_obj);
        Connector c2 = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);
        Assert.IsNotNull(c2);

        Target T_obj = Resources.Load<GameObject>("Prefabs/Target")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }


        c.AssignTargetList(tl);

        c2.AssignLinkedTarget(tl[1]);

        ConnectionInfo info = new ConnectionInfo();
        info.linkedTargetID = 1;


        Assert.AreEqual(c.Dump(), ConnectionInfo.NoConnection());
        Assert.AreEqual(c2.Dump(), info);
    }

    [Test]
    public void Connector_IMP_ConnectToComponent() {
        Connector C_obj = Resources.Load<GameObject>("Prefabs/Square").GetComponentInChildren<Connector>();

        Connector c = MonoBehaviour.Instantiate(C_obj);
        Connector c2 = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);
        Assert.IsNotNull(c2);

        Target T_obj = Resources.Load<GameObject>("Prefabs/Target")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }

        c.AssignTargetList(tl);
        ConnectionInfo info = new ConnectionInfo();
        info.linkedTargetID = 1;

        //c2.ConnectToComponent(c, info);

        //ConnectionInfo c2info = c2.Dump();
        //Debug.Log(c2info.linkedTargetID);
        //Debug.Log(c2info.connectorRotation);

        Assert.AreEqual(c.Dump(), ConnectionInfo.NoConnection());
       // Assert.AreEqual(c2.Dump(), info);
    }
    [Test]
    public void Connector_IMP_GetAvailableConnector() {
        Connector C_obj = Resources.Load<GameObject>("Prefabs/Square").GetComponentInChildren<Connector>();

        Connector c = MonoBehaviour.Instantiate(C_obj);
        Connector c2 = MonoBehaviour.Instantiate(C_obj);
        Assert.IsNotNull(c);
        Assert.IsNotNull(c2);

        Target T_obj = Resources.Load<GameObject>("Prefabs/Target")?.GetComponent<Target>();
        List<Target> tl = new List<Target>();
        for (int i = 0; i < 4; ++i) {
            tl.Add(MonoBehaviour.Instantiate(T_obj));
            Assert.IsNotNull(tl[i]);
        }

        c.AssignTargetList(tl);

        c2.AssignDetectedTarget(tl[2]);

        //(IConnector ic, int tid) = c2.GetAvailableConnector();

        //Assert.AreEqual(tid, 2);
        //Assert.AreEqual(c, ic?.GetTargetObjByIndex(tid)?.GetComponent<Target>()?.ownerConnector);
    }

    [Test]

    public void SelectTest()
    {
        Connector c = Resources.Load<GameObject>("Prefabs/Square").GetComponentInChildren<Connector>();
        Connector C_obj = MonoBehaviour.Instantiate(c);

        Assert.True(C_obj.GetTargetList().Count == 4);
        foreach (var i in C_obj.GetTargetList())
        {
            Assert.False(i.gameObject.activeInHierarchy);
        }
        C_obj.SwitchSelecting(true);
        var count = 0;
        foreach (var i in C_obj.GetTargetList())
        {
            if (i.gameObject.activeInHierarchy)
            {
                count += 1;
            }
        }
        Assert.AreEqual(count, 0);
        C_obj.SetConnectMode(true);
        foreach (var i in C_obj.GetTargetList())
        {
            if (i.gameObject.activeInHierarchy)
            {
                Debug.Log("+1");
                count += 1;
            }
        }
        C_obj.SwitchSelecting(true);
        count = 0;
        foreach (var i in C_obj.GetTargetList())
        {
            if (i.gameObject.activeInHierarchy)
            {
                count += 1;
            }
        }
        Assert.AreEqual(count, 0);
    }
}
