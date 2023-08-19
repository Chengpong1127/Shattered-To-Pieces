using System;
public interface IEntityTriggerable{
    public event Action<Entity> OnTriggerEntity;
}