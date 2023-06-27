using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameComponentFactory{
    /// <summary>
    /// Create a game component object by its GUID. The method will create a new game object set the componentGUID of the component.
    /// </summary>
    /// <param name="id">The GUID of the component.</param>
    /// <returns>The created component.</returns>
    IGameComponent CreateGameComponentObject(int id);
}
