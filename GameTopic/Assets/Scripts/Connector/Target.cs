using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * 用於被 Connector 在連接模式中瞄準的連接點，也負責記錄哪一個 Connector 連接在他身上
 * 在 Connector 中以子物件存在，必須置於擁有 Connector 的 GameObject 底下，
 * 在其他 Connector 連接時會參考 Target 所有物件相對於父物件的 position 作為 Joint 連接時的座標。
 */
public class Target : MonoBehaviour, ITarget
{
    public int targetID { get; set; }

    public IConnector ownerIConnector { get; set; }
    public Connector ownerConnector;
    IConnector aimerConnector;

    private void Start()
    {
        aimerConnector = null;
    }


    int Dump()
    {
        return targetID;
    }

    void Load(int tid)
    {
        targetID = tid;
    }


    public void SetOwner(Connector oc)
    {
        ownerConnector = oc;
    }

    public void SwitchActive(bool b)
    {
        // if (aimerConnector != null) { return; }
        this.gameObject.SetActive(b);
    }

    /*
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
    */

    // interface imp.
    public void ActiveITarget(bool active)
    {
        this.SwitchActive(active);
    }
    public void LinkTarget(IConnector lic)
    {
        if(lic == null) { return; }
        UnLinkTarget();
        SwitchActive(false);
        aimerConnector = lic;
        ownerConnector.AddLinkSelectListener(aimerConnector.linkSelectAction);
    }
    public void UnLinkTarget()
    {
        if (aimerConnector == null) { return; }
        ownerConnector.RemoveLinkSelectListener(aimerConnector.linkSelectAction);
        aimerConnector = null;
        SwitchActive(true);
    }
}
