
public interface IGameComponentFactory{
    /// <summary>
    /// Create a game component object by its GUID. The method will create a new game object set the componentGUID of the component.
    /// </summary>
    /// <param name="id">The GUID of the component.</param>
    /// <returns>The created component.</returns>
    IGameComponent CreateGameComponentObject(int id);
    /// <summary>
    /// Create a game component object by its name. The method will create a new game object set the ComponentName of the component.
    /// </summary>
    /// <param name="gameComponentName"></param>
    /// <returns></returns>
    IGameComponent CreateGameComponentObject(string gameComponentName);
}
