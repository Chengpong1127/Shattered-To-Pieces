using AttributeSystem.Authoring;
using AttributeSystem.Components;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : NetworkBehaviour
{
    [SerializeField] Image BloodBar;
    [SerializeField] AttributeScriptableObject HealthAttribute;
    [SerializeField] AttributeScriptableObject MaxHealthAttribute;
    AttributeSystemComponent ASC;
    AttributeValue AttributeGetter;
    float currentVal;
    float maximaVal;
    float proportion;

    GamePlayer player;

    private void Start() {
        if (!IsOwner) { gameObject.transform.parent.gameObject.SetActive(false); }

        if (!IsServer) { return; }
        GameEvents.AttributeEvents.OnEntityHealthChanged += UpdateBloodBar;
    }

    [ClientRpc]
    public void SetBloodBarLength_ClientRpc(float progress) {

        if (progress < 0) progress = 0;
        if(progress > 1) progress = 1;

        BloodBar.fillAmount = progress;
    }

    public void UpdateBloodBar(Entity entity, float prevHealth, float currentHealth) {
        if (!IsServer) { return; }
        
        player = GetComponentInParent<GamePlayer>();
        Entity ctrlRoomEntity = player?.GetTracedTransform()?.GetComponent<Entity>();

        if (HealthAttribute == null ||
            MaxHealthAttribute == null ||
            ctrlRoomEntity == null ||
            ctrlRoomEntity != entity) { return; }

        ASC = entity.GetComponent<AttributeSystemComponent>();
        if(ASC == null) { return; }

        ASC.GetAttributeValue(HealthAttribute, out AttributeGetter);
        currentVal = AttributeGetter.CurrentValue;
        ASC.GetAttributeValue(MaxHealthAttribute, out AttributeGetter);
        maximaVal = AttributeGetter.CurrentValue;
        proportion = currentVal / maximaVal;

        SetBloodBarLength_ClientRpc(proportion);
    }
}
