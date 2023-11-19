using System.Linq;
using DG.Tweening;
using UnityEngine;

public class OutlineController : MonoBehaviour
{
    private SpriteRenderer[] allSpriteRenderers;
    private Material[] originalMaterials;
    private Material outlineMaterial;
    private Tweener[] tweeners;
    private bool isOutline = false;
    void Awake()
    {
        allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        outlineMaterial = ResourceManager.Instance.LoadMaterial("Outline");
    }

    public void SetOutline(bool isOutline)
    {
        if (this.isOutline == isOutline) return;
        this.isOutline = isOutline;
        if (isOutline)
        {
            originalMaterials = allSpriteRenderers.Select(sr => sr.material).ToArray();
            allSpriteRenderers.ToList().ForEach(sr => sr.material = outlineMaterial);
            StartOutlineAnimation();
        }
        else
        {
            StopOutlineAnimation();
            allSpriteRenderers.ToList().ForEach(sr => sr.material = originalMaterials[allSpriteRenderers.ToList().IndexOf(sr)]);
        }

    }
    private void StartOutlineAnimation(){
        tweeners = allSpriteRenderers.Select(sr => sr.material.DOFloat(360, "_Angle", 2).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart)).ToArray();
    }
    private void StopOutlineAnimation(){
        tweeners.ToList().ForEach(t => t.Kill());
    }


}