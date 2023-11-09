using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BasicSceneLoader: BaseSceneLoader{
    public Image Panel;
    public Slider LoadingSlider;
    public float FadeInDuration = 0.5f;
    public float FadeOutDuration = 0.5f;
    protected override void Awake() {
        base.Awake();
        Panel.gameObject.SetActive(false);
        LoadingSlider.enabled = false;
    }
    public override async void LoadScene(AsyncOperation asyncOperation){
        Panel.gameObject.SetActive(true);
        Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, 0);
        await DOTween.ToAlpha(() => Panel.color, color => Panel.color = color, 1, FadeInDuration).ToUniTask();
        LoadingSlider.enabled = true;
        while (!asyncOperation.isDone){
            LoadingSlider.value = asyncOperation.progress;
            await UniTask.Yield();
        }
        LoadingSlider.enabled = false;
        Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, 1);
        await DOTween.ToAlpha(() => Panel.color, color => Panel.color = color, 0, FadeOutDuration).ToUniTask();
        Panel.gameObject.SetActive(false);
    }
}