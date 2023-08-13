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
    public Connector OwnerConnector { get; set; } = null;
    public bool IsConnected { get => AimerConnector != null; }

    private Connector AimerConnector = null;
    private Renderer Renderer = null;

    private void Awake() {
        Renderer = GetComponent<Renderer>();
        if (Renderer == null) {
            Debug.LogWarning("Target: Renderer is null");
        }
    }

    public void SetOwner(Connector oc)
    {
        OwnerConnector = oc;
    }
    public void LinkTo(Connector lic)
    {
        AimerConnector = lic ?? throw new System.ArgumentNullException("lic");
    }
    public void Unlink()
    {
        AimerConnector = null;
    }

    public void SetTargetDisplay(bool b)
    {
        Renderer.enabled = b;
    }

}
