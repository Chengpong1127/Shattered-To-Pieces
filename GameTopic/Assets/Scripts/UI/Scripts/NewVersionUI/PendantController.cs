using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PendantController : MonoBehaviour
{
    [SerializeField]
    private GameObject ExitPanelPrefab;
    private JumpOutPanel ExitPanel;
    void Awake()
    {
        ExitPanel = Instantiate(ExitPanelPrefab, transform.parent).GetComponent<JumpOutPanel>();
        ExitPanel.OnSendMessage += SendMessageHandler;
    }
    private void SendMessageHandler(string message)
    {
        switch (message)
        {
            case "Confirm":
                LocalPlayerManager.RoomInstance.ExitGame();
                break;
            case "Cancel":
                ExitPanel.ExitScene();
                break;
            default:
                break;
        }
    }
    public void OnSettingClick()
    {
        Debug.Log("Setting");
    }
    public void OnExitClick()
    {
        ExitPanel.EnterScene();
    }
    public void OnDuckClick()
    {
        Debug.Log("Duck");
    }
}
