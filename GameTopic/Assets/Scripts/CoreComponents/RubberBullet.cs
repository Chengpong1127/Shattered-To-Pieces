using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBullet : MonoBehaviour
{
    [SerializeField]
    public GameObject Rubber;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Square")
        {
            Instantiate(Rubber,this.transform.position,Rubber.transform.rotation);
        }
        Destroy(this.gameObject);
    }
}
