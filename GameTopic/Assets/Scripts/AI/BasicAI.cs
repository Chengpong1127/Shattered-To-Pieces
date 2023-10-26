

using UnityEngine;
using NPBehave;
using Cysharp.Threading.Tasks;
using System.Linq;

public class BasicAI : BaseAIAgent
{
    public override Root GetBehaviorTree()
    {
        return new Root(
            new Service(0.5f, UpdateBlackboard,
                new Selector(
                    new BlackboardCondition("TargetPlayer", Operator.IS_SET, Stops.IMMEDIATE_RESTART,
                        new Action(() => {
                            if(BehaviorTree.Blackboard["TargetPlayer"] == null){
                                Debug.Log("TargetPlayer is null");
                            }
                            else{
                                Debug.Log("TargetPlayer is not null");
                            }
                        }
                        )
                            
                    ),
                    new Action(() => Debug.Log("TargetPlayer is not set"))
                )
            )
        );
    }

    protected override void UpdateBlackboard()
    {
        BehaviorTree.Blackboard["TargetPlayer"] = GetClosestEntity(10, "Player");
        base.UpdateBlackboard();
    }
    public void Jump(){
        BodyRigidbody.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
    }
    public void Move(float direction){
        BodyRigidbody.velocity = new Vector2(direction * 5, BodyRigidbody.velocity.y);
    }

}