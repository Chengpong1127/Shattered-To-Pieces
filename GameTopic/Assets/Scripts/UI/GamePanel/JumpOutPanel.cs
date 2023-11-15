using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class JumpOutPanel : BaseGamePanel
{
    public CanvasGroup Background;
    public Transform PanelTransform;
    protected override void Awake()
    {
        base.Awake();
        PanelTransform.localScale = Vector3.zero;
        Background.alpha = 0;
        Background.interactable = false;
        Background.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
    protected override async UniTask EnterSceneAnimation(){
        gameObject.SetActive(true);
        Background.interactable = true;
        Background.blocksRaycasts = true;
        await UniTask.WhenAll(
            Background.DOFade(1, 0.2f).ToUniTask(),
            PanelTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).ToUniTask()
        );
    }
    protected override async UniTask ExitSceneAnimation(){
        await UniTask.WhenAll(
            Background.DOFade(0, 0.2f).ToUniTask(),
            PanelTransform.DOScale(0, 0.5f).SetEase(Ease.InBack).ToUniTask()
        );
        Background.interactable = false;
        Background.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}