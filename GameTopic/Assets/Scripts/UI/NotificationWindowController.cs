using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public class NotificationWindowController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _titleText;
    [SerializeField]
    private TMP_Text _messageText;
    [SerializeField]
    private GameWidget _widget;

    void Awake()
    {
        Debug.Assert(_titleText != null);
        Debug.Assert(_messageText != null);
        Debug.Assert(_widget != null);
    }

    public async UniTask ShowNotification(string title, string message)
    {
        _titleText.text = title;
        _messageText.text = message;
        _widget.Show();
        await UniTask.WaitUntil(() => _widget.IsClose);
    }

}