using System;
public interface IAbilityRebinder
{
    public void StartRebinding(int abilityButtonID);
    public void CancelRebinding();
    public event Action<string> OnFinishRebinding;
}
