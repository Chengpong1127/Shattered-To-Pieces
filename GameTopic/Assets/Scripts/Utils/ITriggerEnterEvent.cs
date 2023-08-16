using System;
public interface ITriggerEntity{
    public event Action<Entity> OnTriggerEnterEvent;
}