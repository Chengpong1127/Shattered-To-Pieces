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
    public int TargetID { get; set; }
    public GameObject targetPoint { get; set; }
    public Connector ownerConnector { get; set; } = null;

    Connector aimerConnector { get; set; } = null;

    private void Awake()
    {
        aimerConnector = null;
        targetPoint = gameObject;

        targetPoint.SetActive(false);
    }


    public void SetOwner(Connector oc)
    {
        ownerConnector = oc;
    }

    public void SwitchActive(bool b)
    {
        if (aimerConnector != null) {  return; }
        this.gameObject.SetActive(b);
    }

    public bool LinkToTarget(Connector lic)
    {
        if(lic == null) { return false; }
        if(aimerConnector != null) { return false; }
        UnLinkToTarget();
        SwitchActive(false);
        aimerConnector = lic;

        return true;
    }
    public void UnLinkToTarget()
    {
        if (aimerConnector == null) { return; }
        aimerConnector = null;
        SwitchActive(true);
    }

}
