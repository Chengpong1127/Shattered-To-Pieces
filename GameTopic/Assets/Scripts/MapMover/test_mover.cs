using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_mover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position=new Vector3(transform.position.x, transform.position.y + 0.05f);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            this.transform.position = new Vector3(transform.position.x, transform.position.y -0.05f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.position = new Vector3(transform.position.x + 0.05f, transform.position.y );

        }
        else if (Input.GetKey(KeyCode.A))
        {
            this.transform.position = new Vector3(transform.position.x - 0.05f, transform.position.y);
        }

    }
}
