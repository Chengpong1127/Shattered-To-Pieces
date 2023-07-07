using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TempAbilityInputUI : MonoBehaviour
{
    public GameObject ButtonPrefab;
    public AbilityInputManager abilityInputManager = new AbilityInputManager();
    public Canvas canvas;
    private GameObject root;
    private void Start() {
        Debug.Assert(ButtonPrefab != null, "ButtonPrefab is null");
        Debug.Assert(abilityInputManager != null, "abilityInputManager is null");
        Debug.Assert(canvas != null, "canvas is null");
        root = new GameObject("AbilityInputUI");
        root.transform.SetParent(canvas.transform);

        for (int i = 0; i < abilityInputManager.AbilityInputEntries.Count; i++)
        {
            var entry = abilityInputManager.AbilityInputEntries[i];
            var entryTransform = createEntry(i, entry);
            entryTransform.SetParent(root.transform);
            entryTransform.position = new Vector3(i * 50, 0, 0);

        }
        root.transform.position = new Vector3(100, 500, 0);
    }
    private Transform createEntry(int index, AbilityInputEntry entry){
        var entryGameObject = new GameObject("AbilityInputEntry");
        entryGameObject.transform.SetParent(root.transform);
        var baseButton = Instantiate(ButtonPrefab, entryGameObject.transform);
        baseButton.GetComponentInChildren<TMP_Text>().text = index.ToString();
        baseButton.GetComponent<Button>().onClick.AddListener(() => entry.RunAllAbilities());

        for(int i = 0; i < entry.Abilities.Count; i++){
            var ability = entry.Abilities[i];
            var button = Instantiate(ButtonPrefab, entryGameObject.transform);
            if(ability == null){
                button.GetComponentInChildren<TMP_Text>().text = "";
            }else{
                button.GetComponentInChildren<TMP_Text>().text = ability.name;
            button.GetComponent<Button>().onClick.AddListener(() => ability.action());
            }
            button.transform.position = new Vector3(0, -50 * (i + 1), 0);
        }


        return entryGameObject.transform;
    }

}
