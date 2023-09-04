using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
        var entity = collision.GetComponent<Entity>();
        if (entity is BaseCoreComponent&&entity.transform.parent.name.Contains("ControlRoom")&&Vector2.Distance(collision.transform.position,transform.position)>0.5f)
        {
            player = entity.transform.parent.gameObject;
            player.transform.position = destination.position;
        }
    }
}
