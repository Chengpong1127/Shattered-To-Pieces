using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// The meta data of the game component.
/// </summary>
[CreateAssetMenu(fileName = "GameComponentData", menuName = "Core/GameComponentData")]
public class GameComponentData : ScriptableObject
{
    /// <summary>
    /// The display name of the game component. Which can be used to display the game component's name.
    /// </summary>
    public string DisplayName;

    /// <summary>
    /// The image of the game component. Which can be used to display the game component's image.
    /// </summary>
    public Image DisplayImage;

    /// <summary>
    /// The Description of the game component. Which can be used to display the game component's description.
    /// </summary>
    public string Description;

    /// <summary>
    /// The price of the game component. Which can be used to display the game component's price.
    /// </summary>
    public int Price;

    /// <summary>
    /// The type of the game component. Which can be used to classify the game component.
    /// </summary>
    public GameComponentType Type;

    /// <summary>
    /// The path of the game component's prefab. Which can be used to instantiate the game component.
    /// </summary>
    public string ResourcePath;
}
