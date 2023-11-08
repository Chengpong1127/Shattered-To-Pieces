using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
public class EnergyDisplay : NetworkBehaviour
{
    public EnergyManager energyManager;
    public Image energyBar;
    private void Start() {
        if (!IsOwner)
        {
            energyBar.enabled=false;
        }
    }
    private void Update() {
        if (IsOwner){
            energyBar.fillAmount = energyManager.Energy.Value / 10;
        }
    }


}