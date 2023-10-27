

using UnityEngine;
using NPBehave;
using Cysharp.Threading.Tasks;
using System.Linq;
using AbilitySystem.Authoring;

public class BasicAI : BaseAIAgent
{
    public float JumpForce = 10;
    public float MoveSpeed = 0.3f;
    public float AttackRange = 3;
    public float AttackCooldown = 1;
    public GameplayEffectScriptableObject AttackEffect;
    public override Root GetBehaviorTree()
    {
        return new Root(
            new Service(1f, UpdateBlackboard,
                new BlackboardCondition("TargetPlayer", Operator.IS_NOT_EQUAL, "null", Stops.IMMEDIATE_RESTART,
                    new Selector(
                        new BlackboardCondition("TargetPlayerDistance", Operator.IS_GREATER, AttackRange, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(ChasePlayer)
                            )
                        ),
                        new BlackboardCondition("TargetPlayerDistance", Operator.IS_SMALLER_OR_EQUAL, AttackRange, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                new Action(Attack),
                                new Wait(AttackCooldown)
                            )
                        )
                    )
                )
            )
        );
    }

    protected override void UpdateBlackboard()
    {
        var TargetPlayer = GetClosestEntity(10, "GameComponent");
        if (TargetPlayer != null) BehaviorTree.Blackboard["TargetPlayer"] = TargetPlayer;
        else BehaviorTree.Blackboard["TargetPlayer"] = "null";
        BehaviorTree.Blackboard["TargetPlayerDistance"] = Vector2.Distance(BodyTransform.position, (BehaviorTree.Blackboard["TargetPlayer"] as Entity)?.BodyTransform.position ?? Vector2.zero);
        BehaviorTree.Blackboard["TargetPlayerPosition"] = (BehaviorTree.Blackboard["TargetPlayer"] as Entity)?.BodyTransform.position ?? Vector2.zero;
        base.UpdateBlackboard();
    }
    protected Action.Result ChasePlayer(bool shouldCancel = false){
        if (shouldCancel) return Action.Result.FAILED;
        if ((float)BehaviorTree.Blackboard["TargetPlayerDistance"] <= AttackRange) return Action.Result.SUCCESS;
        var targetPlayer = BehaviorTree.Blackboard["TargetPlayer"] as Entity;
        Move(targetPlayer.transform.position.x > transform.position.x ? 1 : -1);
        return Action.Result.PROGRESS;
    }

    public void Jump(){
        BodyRigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }
    public void Move(float direction){
        BodyRigidbody.velocity = new Vector2(direction * MoveSpeed, BodyRigidbody.velocity.y);
    }
    public Action.Result Attack(bool shouldCancel = false){
        if (shouldCancel) return Action.Result.FAILED;
        var targetPlayer = BehaviorTree.Blackboard["TargetPlayer"] as Entity;
        if (targetPlayer == null) return Action.Result.FAILED;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(this, targetPlayer, AttackEffect);
        return Action.Result.SUCCESS;
    }



}