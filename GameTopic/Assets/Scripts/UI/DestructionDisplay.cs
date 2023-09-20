using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DestructionDisplay : MonoBehaviour
{
    [SerializeField] AttributeScriptableObject HealthAttribute;
    [SerializeField] AttributeScriptableObject MaxHealthAttribute;
    [SerializeField] bool isDisplay;

    [SerializeField] List<SpriteRenderer> RendererList;
    List<Material> MaterialList = new List<Material>();

    AttributeSystemComponent ASC;
    AttributeValue AttributeGetter;
    Entity entity;
    float currentVal;
    float maximaVal;
    float proportion;

    

    private void Start() {
        RendererList.ForEach(r => {
            MaterialList.AddRange(r.materials);
        });
        if (MaterialList.Count == 0) { isDisplay = false; }

        CheckDisplay();
        UpdateDisplay();
    }

    private void OnEnable() {
        GameEvents.AttributeEvents.OnEntityHealthChanged += OnEntityHealthChanged;
    }
    private void OnDisable() {
        GameEvents.AttributeEvents.OnEntityHealthChanged -= OnEntityHealthChanged;
    }
    private void OnEntityHealthChanged(Entity entity, float prevHealth, float currentHealth) {
        if (entity.Equals(this.entity)) {
            UpdateDisplay();
        }
    }

    void CheckDisplay() {
        ASC = GetComponent<AttributeSystemComponent>();
        entity = GetComponent<Entity>();
        if (HealthAttribute == null ||
            MaxHealthAttribute == null ||
            RendererList == null ||
            ASC == null ||
            entity == null) { isDisplay = false; return; }
    }
    private void UpdateDisplay() {
        if (!isDisplay) { return; }

        ASC.GetAttributeValue(HealthAttribute, out AttributeGetter);
        currentVal = AttributeGetter.CurrentValue;
        ASC.GetAttributeValue(MaxHealthAttribute, out AttributeGetter);
        maximaVal = AttributeGetter.CurrentValue;
        proportion = 1 - (currentVal / maximaVal);

        MaterialList.ForEach(m => {
            m.SetFloat("Fade", proportion);
        });
    }
}
