

using UnityEngine;
using NPBehave;
using Cysharp.Threading.Tasks;
using System.Linq;
using AbilitySystem.Authoring;

public class BasicAI : BaseAIAgent
{
    public float JumpForce = 10;
    public float MoveSpeed = 0.3f;
    public float ChaseRange = 10;
    public float AttackRange = 3;
    public float AttackCooldown = 1;
    public GameplayEffectScriptableObject AttackEffect;
    public override Root GetBehaviorTree()
    {
        return new Root(
            new Service(1f, UpdateBlackboard,
                new Selector(
                    new BlackboardCondition("AttackRangePlayer", Operator.IS_NOT_EQUAL, "null", Stops.IMMEDIATE_RESTART,
                        new Sequence(
                            new Action(Attack),
                            new Wait(AttackCooldown)
                        )
                    ),
                    new BlackboardCondition("TargetPlayer", Operator.IS_NOT_EQUAL, "null", Stops.IMMEDIATE_RESTART,
                        new Sequence(
                            new Action(ChasePlayer)
                        )
                    ),
                    new Action(() => Move(0))
                )
            )
        );
    }

    protected override void UpdateBlackboard()
    {
        var TargetPlayer = GetClosestEntity(ChaseRange, "GameComponent");
        var AttackRangePlayer = GetClosestEntity(AttackRange, "GameComponent");
        if (TargetPlayer != null) BehaviorTree.Blackboard["TargetPlayer"] = TargetPlayer;
        else BehaviorTree.Blackboard["TargetPlayer"] = "null";
        if (AttackRangePlayer != null) BehaviorTree.Blackboard["AttackRangePlayer"] = AttackRangePlayer;
        else BehaviorTree.Blackboard["AttackRangePlayer"] = "null";
        base.UpdateBlackboard();
    }
    protected Action.Result ChasePlayer(bool shouldCancel = false){
        if (shouldCancel) return Action.Result.FAILED;
        var targetPlayer = BehaviorTree.Blackboard["TargetPlayer"] as Entity;
        if (BehaviorTree.Blackboard["AttackRangePlayer"].ToString() != "null") return Action.Result.SUCCESS;
        Move(targetPlayer.BodyTransform.position.x > BodyTransform.position.x ? 1 : -1);
        return Action.Result.PROGRESS;
    }
    public void Move(float direction){
        BodyRigidbody.velocity = new Vector2(direction * MoveSpeed, BodyRigidbody.velocity.y);
    }
    public Action.Result Attack(bool shouldCancel = false){
        if (shouldCancel) return Action.Result.FAILED;
        var targetPlayer = BehaviorTree.Blackboard["AttackRangePlayer"] as Entity;
        if (targetPlayer == null) return Action.Result.FAILED;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(this, targetPlayer, AttackEffect);
        return Action.Result.SUCCESS;
    }



}