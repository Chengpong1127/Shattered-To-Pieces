using System.Collections.Generic;

public abstract class Entity: BaseEntity{
    Dictionary<EntityStatusType, IStatus> _statuses = new Dictionary<EntityStatusType, IStatus>();
    public BuffManager BuffManager= new BuffManager();
    public IStatus GetStatus(EntityStatusType type) {
        return _statuses.ContainsKey(type) ? _statuses[type] : null;
    }
}

public enum EntityStatusType {

}

public interface IStatus {
    EntityStatusType Type { get; }
}

