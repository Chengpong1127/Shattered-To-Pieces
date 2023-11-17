using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
public class RainbowText : MonoBehaviour
{
    public float speed;  

    private Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<Text>();
    }
    public void StarRainbow()
    {
        StartCoroutine(RainbowColorChange());
    }
    IEnumerator RainbowColorChange()
    {
        float startTime = Time.time;

        while (true)
        {
            float offset = (Time.time - startTime) * speed;

            float lerpValue = Mathf.Sin(offset) * 0.5f + 0.5f;


            Color rainbowColor = Color.HSVToRGB(lerpValue, 1.0f, 1.0f);

            textMesh.color = rainbowColor;

            yield return null;
        }
    }
}
