using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberComponent : MonoBehaviour
{
    [SerializeField]
    private TextMesh textMesh;

    public float Alpha;
    public float yOffset;
    private float startYPosition;
    public void Initialise(float number)
    {
        startYPosition = transform.position.y;
        if (number < 0)
        {
            number *= -1;
            textMesh.color = Color.red;
        }
        else
        {
            textMesh.color = Color.green;
        }

        var colour = textMesh.color;
        colour.a = 0;
        textMesh.color = colour;

        textMesh.text = number.ToString("0");
       // StartCoroutine(Fly());
    }

    public IEnumerator Fly()
    {
        var colour = textMesh.color;
        while (colour.a > 0)
        {
            colour.a = Alpha;
            textMesh.color = colour;
            var position = transform.position;
            position.y = startYPosition + yOffset;
            transform.position = position;
            yield return null;
        }
        Destroy(this.gameObject);
        yield return null;
    }
    public void Awake()
    {
        //StartCoroutine(Fly());  
    }
    public void Update()
    {

        var colour = textMesh.color;
        colour.a = Alpha;
        textMesh.color = colour;
        var position = transform.position;
        position.y = startYPosition + yOffset;
        transform.position = position;
        Debug.Log(colour.a);
        if (colour.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
