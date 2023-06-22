using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 用於被 Connector 在連接模式中瞄準的連接點，也負責記錄哪一個 Connector 連接在他身上
 * 在 Connector 中以子物件存在，必須置於擁有 Connector 的 GameObject 底下，
 * 在其他 Connector 連接時會參考 Target 所有物件相對於父物件的 position 作為 Joint 連接時的座標。
 */
public class Target : MonoBehaviour
{

    public int targetID { get; set; }
    public GameObject targetPoint { get; set; }
    public Connector ownerConnector { get; set; }

    Connector aimerConnector { get; set; }

    private void Awake()
    {
        aimerConnector = null;
        aimerRBonnector = null; // for RBconnector
        targetPoint = gameObject;

        targetPoint.SetActive(false);
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


    // for RBconnector
    public RBconnector ownerRBconnector { get; set; }
    public RBconnector aimerRBonnector { get; set; }

    public bool RB_LinkToTarget(RBconnector lic) {
        if (lic == null) { return false; }
        if (aimerRBonnector != null) { return false; }
        RB_UnLinkToTarget();
        RB_SwitchActive(false);
        aimerRBonnector = lic;

        return true;
    }
    public void RB_UnLinkToTarget() {
        if (aimerRBonnector == null) { return; }
        aimerRBonnector = null;
        RB_SwitchActive(true);
    }

    public void  RB_SwitchActive(bool b) {
        if (aimerRBonnector != null) { return; }
        this.gameObject.SetActive(b);
    }
}
