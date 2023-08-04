
public interface IGameComponentFactory{
    /// <summary>
    /// Create a game component object by its name. The method will create a new game object set the ComponentName of the component.
    /// </summary>
    /// <param name="gameComponentName"></param>
    /// <returns></returns>
    IGameComponent CreateGameComponentObject(string gameComponentName);
}
