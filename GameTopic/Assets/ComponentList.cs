using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ComponentList : MonoBehaviour
{
    public GameObject car;
    private Dictionary<String, int> prefabCounts;
    private PlayerController playerController;
    private void Start()
    {

        playerController= new PlayerController();
        prefabCounts= new Dictionary<String, int>();
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            string prefabName = button.GetComponentInChildren<TextMeshProUGUI>().text;
            if (prefabName == "save")
            {
                button.onClick.AddListener(() =>
                {
                    if (playerController != null)
                    {
                        playerController.SaveComponentCount(prefabCounts);
                    }
                    else
                    {
                        Debug.Log("PlayerController not assigned.");
                    }
                });
            }
            else if (prefabName == "load")
            {
          
                button.onClick.AddListener(() =>
                {
                    foreach (Transform child in car.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    prefabCounts =playerController.LoadComponentCount();
                    //Debug.Log("prefabCounts: " + (prefabCounts == null ? "null" : prefabCounts.Count.ToString()));
                    foreach (var kvp in prefabCounts)
                    {
                        String prefabName = kvp.Key;
                        int count = kvp.Value;
                        GameObject prefab = Resources.Load<GameObject>(prefabName);
                        for (int i = 0; i < count; i++)
                        {
                            GameObject newObject = Instantiate(prefab, car.transform);
                            newObject.name = prefab.name;
                        }
                    }
                });

            }
            else
            {
                button.onClick.AddListener(() =>
                {
                    GameObject prefab = Resources.Load<GameObject>(prefabName);

                    if (prefab != null)
                    {
                        GameObject newObject = Instantiate(prefab, car.transform);
                        newObject.name = prefabName;
                        if (prefabCounts.ContainsKey(prefabName))
                        {
                            prefabCounts[prefabName]++;
                        }
                        else
                        {
                            prefabCounts.Add(prefabName, 1);
                        }
                    }
                    else
                    {
                        Debug.Log("Prefab not found: " + prefabName);
                    }
                });
            }
           
        }
    }
}
