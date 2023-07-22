using System.Collections.Generic;

public interface IDevice: IStorable{
    public IGameComponent RootGameComponent { set; get; }
    /// <summary>
    /// Get all of the ability list of this device.
    /// </summary>
    /// <returns></returns>
    public List<Ability> getAbilityList();
}
