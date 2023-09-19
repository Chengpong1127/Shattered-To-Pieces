using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class RubberBullet : MonoBehaviour
{
    [SerializeField]
    public GameObject Rubber;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Square"||collision.gameObject.tag=="Ground")
        {
            var rubber=Instantiate(Rubber,this.transform.position,Rubber.transform.rotation);
            rubber.GetComponent<NetworkObject>().Spawn();
            Destroy(this.transform.parent.gameObject);
            this.transform.parent.GetComponent<NetworkObject>().Despawn();
        }
        else
        {
            return;
        }

    }
}
