using System;
public interface IAbilityKeyChanger
{
    public void StartChangeAbilityKey(int abilityButtonID);
    public void EndChangeAbilityKey();
    public event Action<string> OnFinishChangeAbility;
}
