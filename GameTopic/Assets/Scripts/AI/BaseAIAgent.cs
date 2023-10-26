using UnityEngine;
using NPBehave;



public abstract class BaseAIAgent: Entity{
    public Root BehaviorTree;
    public abstract Root GetBehaviorTree();
    protected override void Start() {
        base.Start();
        if (IsServer){
            BehaviorTree = GetBehaviorTree();
            BehaviorTree.Start();
        }
    }
}