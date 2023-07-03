using System.Collections;
using System.Collections.Generic;

public class UnitManager
{
    public Dictionary<int, IUnit> UnitMap { get; private set; }
    public UnitManager()
    {
        UnitMap = new Dictionary<int, IUnit>();
    }
    public void AddUnit(IUnit unit)
    {
        UnitMap.Add(unit.UnitID, unit);
    }
    public void RemoveUnit(IUnit unit)
    {
        UnitMap.Remove(unit.UnitID);
    }
    public void RemoveUnit(int unitID)
    {
        UnitMap.Remove(unitID);
    }
    public IUnit GetUnit(int unitID)
    {
        return UnitMap[unitID];
    }
    public bool ContainsUnit(int unitID)
    {
        return UnitMap.ContainsKey(unitID);
    }
    public bool ContainsUnit(IUnit unit)
    {
        return UnitMap.ContainsKey(unit.UnitID);
    }
    public void Clear()
    {
        UnitMap.Clear();
    }
    public int GetNewUnitID()
    {
        var newID = 0;
        while (UnitMap.ContainsKey(newID))
        {
            newID++;
        }
        return newID;
    }
    public void ForEachUnit(System.Action<IUnit> action)
    {
        foreach (var unit in UnitMap.Values)
        {
            action(unit);
        }
    }

}
