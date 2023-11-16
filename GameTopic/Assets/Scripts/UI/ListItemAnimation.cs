using UnityEngine;
using DG.Tweening;

public class ListItemAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform ItemTransform;
    void Awake()
    {
        if (ItemTransform == null)
        {
            ItemTransform = transform;
        }
    }

    public void ShowAnimation()
    {
        ItemTransform.localScale = Vector3.zero;
        ItemTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }
}