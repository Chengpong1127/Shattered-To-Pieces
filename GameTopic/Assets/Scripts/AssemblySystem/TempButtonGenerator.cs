using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TempButtonGenerator : MonoBehaviour
{
    public Canvas canvas;
    public GameObject buttonPrefab;
    private List<GameObject> buttons = new List<GameObject>();

    public void GenerateButtons(List<Ability> abilities){
        clearAllButtons();
        for (int i = 0; i < abilities.Count; i++) {
            var button = Instantiate(buttonPrefab, canvas.transform);
            int index = i; // Create a local variable to capture the current value of i
            button.GetComponent<Button>().onClick.AddListener(() => abilities[index].action());
            button.GetComponentInChildren<TMP_Text>().text = abilities[i].name;
            button.transform.position = new Vector3(100, 600 - i * 50, 0);
            buttons.Add(button);
        }
    }
    public void clearAllButtons(){
        foreach(var button in buttons){
            Destroy(button);
        }
        buttons.Clear();
    }
    
}
