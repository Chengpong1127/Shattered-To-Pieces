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

        c.SetTargetList(tl);

        Assert.AreEqual(c.GetTarget(0), tl[0]);
        Assert.AreEqual(c.GetTarget(1), tl[1]);
        Assert.AreEqual(c.GetTarget(2), tl[2]);
        Assert.AreEqual(c.GetTarget(3), tl[3]);
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

        c.SetTargetList(tl);
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
    }
}
