using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerNameDisplay : NetworkBehaviour
{
    [SerializeField]
    private TMP_Text playerNameText;
    [SerializeField]
    private Color LocalPlayerColor;
    [SerializeField]
    private Color RemotePlayerColor;

    void Awake()
    {
        Debug.Assert(playerNameText != null);
    }

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
        playerNameText.color = IsOwner ? LocalPlayerColor : RemotePlayerColor;
    }
}