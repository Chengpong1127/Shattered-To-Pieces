using System;
public interface IEntityCollisionable{
    public event Action<Entity> OnCollisionEntity;
}