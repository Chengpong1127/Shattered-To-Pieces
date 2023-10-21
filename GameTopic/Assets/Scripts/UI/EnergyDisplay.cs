using TMPro;
using UnityEngine;
using Unity.Netcode;
public class EnergyDisplay : NetworkBehaviour
{
    public TMP_Text energyText;
    public EnergyManager energyManager;
    private void Start() {
        if (!IsOwner){
            energyText.enabled = false;
        }
    }
    private void Update() {
        if (IsOwner){
            energyText.text = ((int)energyManager.Energy.Value).ToString();
        }
    }


}