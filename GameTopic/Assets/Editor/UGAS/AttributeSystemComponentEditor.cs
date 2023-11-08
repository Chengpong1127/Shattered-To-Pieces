using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using AttributeSystem.Components;

[CustomEditor(typeof(AttributeSystemComponent))]
public class AttributeSystemComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (AttributeSystemComponent)target;

        GUILayout.Label("Attribute Values");
        var values = component.GetAttributeDictionaryCopy();
        foreach (var value in values)
        {
            GUILayout.Label($"{value.Key.name}: {value.Value.BaseValue} / {value.Value.CurrentValue}");
        }

    }
}