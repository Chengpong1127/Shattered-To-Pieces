using Cysharp.Threading.Tasks;
using UnityEngine;

public class GarbageCan: GameComponent{
    public Target Target;

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(Target != null, "Target is null");
        Target.OnLinked += OnLinkedHandler;
    }

    protected override void Start()
    {
        base.Start();
        Target.SetTargetDisplay(true);
    }

    private async void OnLinkedHandler(Connector connector)
    {
        await UniTask.WaitForSeconds(0.5f);
        connector.GameComponent.Die();
        Target.SetTargetDisplay(true);
    }
}