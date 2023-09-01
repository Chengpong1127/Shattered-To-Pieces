using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    public Transform destination;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player&&Vector2.Distance(player.transform.position,transform.position)>0.5f)
        {
            player.transform.position = destination.position;
        }
    }
}
