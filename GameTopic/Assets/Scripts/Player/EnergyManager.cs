using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnergyManager : NetworkBehaviour {
    public NetworkVariable<float> Energy = new();
    public float MaxEnergy = 10;
    public float EnergyGainPerSecond = 1;

    public void CostEnergy(float cost){
        Debug.Assert(HasEnergy(cost), "Not enough energy");
        Energy.Value -= cost;
    }

    public void GainEnergy(float gain){
        Energy.Value += gain;
    }

    public bool HasEnergy(float cost){
        return Energy.Value >= cost;
    }

    private void Update() {
        if (IsServer){
            Energy.Value = Mathf.Clamp(Energy.Value + EnergyGainPerSecond * Time.deltaTime, 0, MaxEnergy);
        }
    }
}