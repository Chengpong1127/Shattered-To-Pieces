using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class GameComponentSelectedHandler : MonoBehaviour, IGameEventHandler
{
    public Color TargetColor = new Color(1, 1, 1, 0.5f);
    public float BlinkDuration = 0.5f;
    public AnimationCurve BlinkCurve;
    private Dictionary<GameComponent, CancellationTokenSource> GameComponentToToken = new();
    void OnEnable()
    {
        GameEvents.GameComponentEvents.OnGameComponentSelected += OnGameComponentSelected;
    }
    void OnDisable()
    {
        GameEvents.GameComponentEvents.OnGameComponentSelected -= OnGameComponentSelected;
    }

    public void OnGameComponentSelected(GameComponent gameComponent, bool selected)
    {
        switch (selected)
        {
            case true:
                if (!GameComponentToToken.ContainsKey(gameComponent))
                {
                    var tokenSource = new CancellationTokenSource();
                    GameComponentToToken.Add(gameComponent, tokenSource);
                    StartSelectionAnimation(gameComponent, tokenSource.Token);
                }
                break;
            case false:
                if (GameComponentToToken.ContainsKey(gameComponent))
                {
                    var tokenSource = GameComponentToToken[gameComponent];
                    tokenSource.Cancel();
                    GameComponentToToken.Remove(gameComponent);
                }
                break;
        }
    }

    private void StartSelectionAnimation(GameComponent gameComponent, CancellationToken token)
    {
        var renderers = gameComponent.BodyRenderers;
        renderers.Select(renderer => renderer as SpriteRenderer).
            Where(renderer => renderer != null).
            ToList().ForEach(async renderer => {
                Color originalColor = renderer.color;
                Color targetColor = TargetColor;
                try{
                    while (true)
                    {
                        await RunColorAnimation(renderer, originalColor, targetColor, token);
                    }
                }catch (System.OperationCanceledException){
                    renderer.color = originalColor;
                }
                
            });
    }
    private async UniTask RunColorAnimation(SpriteRenderer renderer, Color originalColor, Color targetColor, CancellationToken token)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            renderer.color = Color.Lerp(originalColor, targetColor, BlinkCurve.Evaluate(elapsedTime));
            elapsedTime += Time.deltaTime / BlinkDuration;
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

}