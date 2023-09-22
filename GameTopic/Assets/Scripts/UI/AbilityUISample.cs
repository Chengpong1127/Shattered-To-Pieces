using Cysharp.Threading.Tasks;
using Unity.Netcode;
public class AbilityUISample : NetworkBehaviour
{
    private BasePlayer player;
    private AbilityManager abilityManager;
    async void Awake()
    {
        player = GetComponent<BasePlayer>();
        await UniTask.WaitUntil(() => player.IsAlive.Value == true);
        if (IsServer){
            abilityManager = player.SelfDevice.AbilityManager;
            GameEvents.AbilityManagerEvents.OnSetAbilityToEntry += (_, _)=> UpdateUI();
            GameEvents.AbilityManagerEvents.OnSetAbilityOutOfEntry += _ => UpdateUI();
        }

    }

    private void UpdateUI(){
        if (IsServer){
            for (int i = 0; i < abilityManager.AbilityInputEntries.Count; i++)
            {
                for(int j = 0; j < abilityManager.AbilityInputEntries[i].Abilities.Count; j++){
                    var ability = abilityManager.AbilityInputEntries[i].Abilities[j];
                    UpdateSingleAbilityUI_ClientRpc(ability.AbilityName, i, j);
                }
            }
        }
    }


    /// <summary>
    /// Update single ability name on client.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="abilityNumber"></param>
    [ClientRpc]
    void UpdateSingleAbilityUI_ClientRpc(string name, int entryID, int entryAbilityIndex){
        if (IsOwner){
            // TODO: Update UI
        }
    }

    /// <summary>
    /// Set Ability To Entry on client. This will be called on UI control.
    /// </summary>
    /// <param name="entryID"></param>
    /// <param name="entryAbilityIndex"></param>
    /// <param name="newEntryID"></param>
    public void SetAbilityToEntry(int entryID, int entryAbilityIndex, int newEntryID){
        if (IsOwner){
            SetAbilityToEntry_ServerRpc(entryID, entryAbilityIndex, newEntryID);
        }
    }
    [ServerRpc]
    private void SetAbilityToEntry_ServerRpc(int entryID, int entryAbilityIndex, int newEntryID){
        var ability = abilityManager.AbilityInputEntries[entryID].Abilities[entryAbilityIndex];
        abilityManager.SetAbilityToEntry(newEntryID, ability);
    }
    [ServerRpc]
    private void SetAbilityOutOfEntry_ServerRpc(int entryID, int entryAbilityIndex){
        var ability = abilityManager.AbilityInputEntries[entryID].Abilities[entryAbilityIndex];
        abilityManager.SetAbilityOutOfEntry(ability);
    }
}