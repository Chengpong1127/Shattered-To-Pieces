using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PendantController : MonoBehaviour
{
    [SerializeField]
    private GameObject ExitPanelPrefab;
    private BackgroundWidget ExitPanel;
    void Awake()
    {
        ExitPanel = Instantiate(ExitPanelPrefab, transform.parent).GetComponent<BackgroundWidget>();
        ExitPanel.OnSendMessage += SendMessageHandler;
        ExitPanel.gameObject.SetActive(false);
    }
    private void SendMessageHandler(string message)
    {
        switch (message)
        {
            case "Confirm":
                LocalPlayerManager.RoomInstance.ExitGame();
                break;
            case "Cancel":
                ExitPanel.Close();
                break;
            default:
                break;
        }
    }
    public void OnExitClick()
    {
        ExitPanel.Show();
    }
    public void OnDuckClick()
    {
        Debug.Log("Duck");
    }
}
