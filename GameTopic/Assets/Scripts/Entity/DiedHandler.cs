using UnityEngine;
[RequireComponent(typeof(BaseEntity))]
public class DiedHandler : MonoBehaviour
{
    public GameObject DiedEffect;
    public int EffectTime = 1;

    void Awake()
    {
        GetComponent<BaseEntity>().OnEntityDied += HandleEntityDied;
    }
    public void HandleEntityDied()
    {
        var effect = Instantiate(DiedEffect, transform.position, Quaternion.identity);
        Destroy(effect, EffectTime);
    }
}