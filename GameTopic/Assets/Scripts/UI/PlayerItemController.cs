using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _playerName;
    [SerializeField]
    private Image ReadyImage;
    [SerializeField]
    private Image UnreadyImage;

    void Awake()
    {
        Debug.Assert(_playerName != null);
        Debug.Assert(ReadyImage != null);
        Debug.Assert(UnreadyImage != null);
    }

    public void SetPlayer(string playerName, bool isReady){
        _playerName.fontStyle = FontStyles.Normal;
        _playerName.text = playerName;
        ReadyImage.enabled = isReady;
        UnreadyImage.enabled = !isReady;
    }

    public void SetLocalPlayer(){
        _playerName.fontStyle = FontStyles.Bold;
    }
}