using UnityEngine;

public class DiedHandler : MonoBehaviour
{
    public GameObject DiedEffect;
    public int EffectTime = 1;
    public void OnDestroy()
    {
        if (DiedEffect != null)
        {
            var effect = Instantiate(DiedEffect, transform.position, Quaternion.identity);
            Destroy(effect, EffectTime);
        }
    }
}