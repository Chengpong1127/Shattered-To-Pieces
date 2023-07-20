using System;
public interface IAbilityKeyChanger
{
    public void StartChangeAbility(int abilityButtonID);
    public void EndChangeAbility();
    public event Action<string> OnFinishChangeAbility;
}
