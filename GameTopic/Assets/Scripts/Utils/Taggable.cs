using UnityEngine;
using GameplayTagNamespace.Authoring;
using System.Collections.Generic;

public class Taggable : MonoBehaviour {
    
    [SerializeField]
    public List<GameplayTag> TagList;

    void Awake()
    {
        if (TagList == null)
        {
            TagList = new List<GameplayTag>();
        }
    }

    public bool HasTag(GameplayTag tag)
    {
        return TagList.Contains(tag);
    }
    public bool HasTag(string tag)
    {
        return TagList.Exists(t => t.TagName == tag);
    }
    public bool HasTag(GameplayTag tag, int nSearchLimit)
    {
        return TagList.Exists(t => t.IsDescendantOf(tag, nSearchLimit));
    }
    public void AddTag(GameplayTag tag)
    {
        if (!TagList.Contains(tag))
        {
            TagList.Add(tag);
        }
    }
    public void RemoveTag(GameplayTag tag)
    {
        if (TagList.Contains(tag))
        {
            TagList.Remove(tag);
        }
    }

    
}