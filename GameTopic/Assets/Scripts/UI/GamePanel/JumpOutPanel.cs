using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class BackgroundWidget : GameWidget
{
    [SerializeField]
    private Image BackgroundImage;
    [SerializeField]
    private Transform MainPanel;

    protected override async UniTask ShowAnimation()
    {
        BackgroundImage.color = new Color(0, 0, 0, 0);
        MainPanel.localScale = Vector3.zero;
        await UniTask.WhenAll(
            BackgroundImage.DOFade(0.5f, Duration).ToUniTask(),
            MainPanel.DOScale(1, Duration).SetEase(EaseType).ToUniTask()
        );
    }
    protected override async UniTask CloseAnimation()
    {
        await UniTask.WhenAll(
            BackgroundImage.DOFade(0, Duration).ToUniTask(),
            MainPanel.DOScale(0, Duration).SetEase(EaseType).ToUniTask()
        );
    }
}