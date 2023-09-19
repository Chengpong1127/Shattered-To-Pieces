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

    void CheckDisplay() {
        ASC = GetComponent<AttributeSystemComponent>();
        if (HealthAttribute == null ||
            MaxHealthAttribute == null ||
            RendererList == null ||
            ASC == null) { isDisplay = false; return; }
    }
    private void UpdateDisplay() {
        if (!isDisplay) { return; }

        ASC.GetAttributeValue(HealthAttribute, out AttributeGetter);
        currentVal = AttributeGetter.CurrentValue;
        ASC.GetAttributeValue(MaxHealthAttribute, out AttributeGetter);
        maximaVal = AttributeGetter.CurrentValue;
        proportion = currentVal / maximaVal;

        MaterialList.ForEach(m => {
            m.SetFloat("Fade", proportion);
        });
    }
}
