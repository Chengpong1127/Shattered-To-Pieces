using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubber : MonoBehaviour
{
    public void Awake()
    {
        StartCoroutine(Stay());
    }
    public IEnumerator Stay()
    {
        yield return new WaitForSeconds(3);
        Destroy(this.gameObject);
        yield return null;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent.name == "ControlRoom(Clone)")
        {
            //©ñ½w³t
            Destroy(this.gameObject);
        }
    }
}
