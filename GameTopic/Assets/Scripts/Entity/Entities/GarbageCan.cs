using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

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
       // this.BodyAnimator.SetBool("IsTrigger",true);
        await UniTask.WaitForSeconds(0.5f);
        connector.GameComponent.Die();
        Target.SetTargetDisplay(true);
        //this.BodyAnimator.SetBool("IsTrigger", false);

    }
    [ClientRpc]
    public override void SetAvailableForConnectionClientRpc(bool available)
    {
    }
}