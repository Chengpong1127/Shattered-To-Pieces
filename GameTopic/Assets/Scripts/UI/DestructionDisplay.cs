using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Netcode;

public class DestructionDisplay : NetworkBehaviour
{
    [SerializeField] AttributeScriptableObject HealthAttribute;
    [SerializeField] AttributeScriptableObject MaxHealthAttribute;
    [SerializeField] bool isDisplay;

    [SerializeField] List<SpriteRenderer> RendererList;
    List<Material> MaterialList = new List<Material>();

    AttributeSystemComponent ASC;
    AttributeValue AttributeGetter;
    Entity entity;

    

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
        var currentVal = AttributeGetter.CurrentValue;
        ASC.GetAttributeValue(MaxHealthAttribute, out AttributeGetter);
        var maximaVal = AttributeGetter.CurrentValue;
        var proportion = 1 - (currentVal / maximaVal);

        UpdateMaterials_ClientRpc(proportion);
    }
    [ClientRpc]
    private void UpdateMaterials_ClientRpc(float proportion){
        MaterialList.ForEach(m => {
            m.SetFloat("_Fade", proportion);
        });
    }
}
