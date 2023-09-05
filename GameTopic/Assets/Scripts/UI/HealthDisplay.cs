using UnityEngine;
using TMPro;
public class HealthDisplay : MonoBehaviour {
    
    public TextMeshProUGUI healthText;
    public Entity entity;
    private void OnEnable() {
        GameEvents.AttributeEvents.OnEntityHealthChanged += OnEntityHealthChanged;
    }

    private void OnDisable() {
        GameEvents.AttributeEvents.OnEntityHealthChanged -= OnEntityHealthChanged;
    }

    private void OnEntityHealthChanged(Entity entity, float prevHealth, float currentHealth){
        Debug.Log("OnEntityHealthChanged: " + entity);
        if (entity.Equals(this.entity)){
            
            healthText.text = currentHealth.ToString();
        }
    }

}