using UnityEngine;
using NPBehave;
using Cysharp.Threading.Tasks;
using System.Linq;



public abstract class BaseAIAgent: Entity{
    public Root BehaviorTree;
    public abstract Root GetBehaviorTree();
    public override void OnNetworkSpawn() {
        if (IsServer){
            BehaviorTree = GetBehaviorTree();
#if UNITY_EDITOR
            Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
            debugger.BehaviorTree = BehaviorTree;
#endif
            BehaviorTree.Start();
        }
    }
    public override void Die()
    {
        if (BehaviorTree != null && BehaviorTree.CurrentState == Node.State.ACTIVE) BehaviorTree.Stop();
        base.Die();
    }

    protected virtual void UpdateBlackboard()
    {
        var attributeMap = AttributeSystemComponent.GetFullAttributeDictionary();
        attributeMap.ToList().ForEach(pair => BehaviorTree.Blackboard[pair.Key.Name] = pair.Value.CurrentValue);
    }

    protected BaseEntity GetClosestEntity(float radius, params string[] tags){
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        BaseEntity[] entities = colliders
            .Select(collider => collider.GetComponentInParent<BaseEntity>())
            .Where(entity => entity != null)
            .Where(entity => tags.All(tag => entity.Taggable.HasTag(tag))).ToArray();
        if (entities.Length == 0) return null;
        BaseEntity closestEntity = entities[0];
        float closestDistance = Vector2.Distance(transform.position, closestEntity.transform.position);
        for (int i = 1; i < entities.Length; i++)
        {
            float distance = Vector2.Distance(transform.position, entities[i].transform.position);
            if (distance < closestDistance){
                closestDistance = distance;
                closestEntity = entities[i];
            }
        }
        return closestEntity;
    }

    
}