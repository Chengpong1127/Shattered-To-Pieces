using AbilitySystem.Authoring;
using UnityEngine;

public abstract class DisplayableAbilityScriptableObject: AbstractAbilityScriptableObject{
    [Header("Display Info")]
    [SerializeField]
    public Sprite Image;
    [SerializeField]
    public Sprite Icon;
    [SerializeField]
    public string Description;
    [SerializeField]
    public bool IsPlaceImage;

}