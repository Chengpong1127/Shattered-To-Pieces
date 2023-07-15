using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameComponentData", menuName = "Core/GameComponentData")]
public class GameComponentData : ScriptableObject
{
    /// <summary>
    /// The display name of the game component.
    /// </summary>
    public string GameComponentName;

    /// <summary>
    /// The path of the game component's prefab. Which can be used to instantiate the game component.
    /// </summary>
    public string ResourcePath;
}
