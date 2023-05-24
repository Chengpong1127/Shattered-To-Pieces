using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * �Ω�Q Connector �b�s���Ҧ����˷Ǫ��s���I�A�]�t�d�O�����@�� Connector �s���b�L���W
 * �b Connector ���H�l����s�b�A�����m��֦� Connector �� GameObject ���U�A
 * �b��L Connector �s���ɷ|�Ѧ� Target �Ҧ�����۹������� position �@�� Joint �s���ɪ��y�СC
 */
public class Target : MonoBehaviour
{
    public Connector ownerConnector;
    Connector aimerConnector;

    private void Start()
    {
        aimerConnector = null;
    }


    Connector Dump()
    {
        return aimerConnector;
    }

    void Load(Connector ac)
    {
        aimerConnector = ac;
    }


    public void SetOwner(Connector oc)
    {
        ownerConnector = oc;
    }

    public void SwitchActive(bool b)
    {
        if (aimerConnector != null) { return; }
        this.gameObject.SetActive(b);
    }

    public void LinkTarget(Connector lc)
    {
        if(lc == null) { return; }
        UnLinkTarget();
        SwitchActive(false);
        aimerConnector = lc;
        ownerConnector.linkedHandler.AddListener(aimerConnector.SwitchLinkingSelect);
    }

    public void UnLinkTarget()
    {
        if(aimerConnector == null) { return; }
        ownerConnector.linkedHandler.RemoveListener(aimerConnector.SwitchLinkingSelect);
        aimerConnector = null;
        SwitchActive(true);
    }
}
