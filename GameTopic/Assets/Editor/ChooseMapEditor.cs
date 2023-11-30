using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

public class DamageEntityEditor : EditorWindow
{
    [MenuItem("Tools/DamageEntityEditor")]
    static void Init()
    {
        DamageEntityEditor chooseMapEditor = GetWindow<DamageEntityEditor>();
        chooseMapEditor.Show();
    }

    public void CreateGUI(){
        VisualElement root = rootVisualElement;
        var allEntities = GameObject.FindObjectsOfType<Entity>();
        var damageEffect = ResourceManager.Instance.LoadGameplayEffect("SimpleDamage");
        damageEffect.gameplayEffect.Modifiers[0].Multiplier = -50;
        foreach(var entity in allEntities){
            var button = new Button();
            button.text = entity.gameObject.name;
            button.clicked += () => {
                GameEvents.GameEffectManagerEvents.RequestGiveGameEffect(entity, entity, damageEffect);
            };
            root.Add(button);
        }
    }


}