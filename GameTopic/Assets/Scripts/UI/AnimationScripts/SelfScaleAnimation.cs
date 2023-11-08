using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
public class SelfScaleAnimation : MonoBehaviour
{

    public float ScaleTarget = 1.2f;
    public float Duration = 0.5f;
    public Ease Ease = Ease.InOutSine;
    void Start()
    {
        transform.DOScale(ScaleTarget, Duration)
        .SetEase(Ease)
        .SetLoops(-1, LoopType.Yoyo);
    }
    void OnDestroy()
    {
        transform.DOKill();
    }
}