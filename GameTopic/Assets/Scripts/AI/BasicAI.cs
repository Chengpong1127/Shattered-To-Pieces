

using UnityEngine;
using NPBehave;

public class BasicAI : BaseAIAgent
{
    public override Root GetBehaviorTree()
    {
        return new Root(
            new Action(() => {
                Debug.Log("Hello World!");
                return true;
            })
        );
    }

}