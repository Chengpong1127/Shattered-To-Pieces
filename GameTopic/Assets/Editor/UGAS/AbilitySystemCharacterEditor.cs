using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using AbilitySystem;

[CustomEditor(typeof(AbilitySystemCharacter))]
public class AbilitySystemCharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (AbilitySystemCharacter)target;

        GUILayout.Label("Applied Gameplay Effects");
        var AppliedGameplayEffects = component.AppliedGameplayEffects;
        foreach (var gameplayEffect in AppliedGameplayEffects)
        {
            GUILayout.Label($"{gameplayEffect.spec.GameplayEffect.name} - {gameplayEffect.spec.DurationRemaining}");
        }
        

    }
}