using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
public class HealthDisplay : NetworkBehaviour {
    
    public TextMeshProUGUI healthText;
    public Entity entity;
    void Start()
    {
        healthText.alpha = 0.2f;
        healthText.color = Color.white;
    }
    private void OnEnable() {
        GameEvents.AttributeEvents.OnEntityHealthChanged += OnEntityHealthChanged;
    }

    private void OnDisable() {
        GameEvents.AttributeEvents.OnEntityHealthChanged -= OnEntityHealthChanged;
    }

    private void OnEntityHealthChanged(Entity entity, float prevHealth, float currentHealth){
        if (entity.Equals(this.entity)){
            healthText.text = currentHealth.ToString();
            StartTextAnimation();
        }
    }
    private async void StartTextAnimation(){
        healthText.color = Color.red;
        healthText.alpha = 1;
        await UniTask.Delay(100);
        healthText.color = Color.white;
        healthText.alpha = 0.2f;
        
    }



}