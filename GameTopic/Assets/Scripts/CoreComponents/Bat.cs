using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities { get; private set; } = new Dictionary<string, Ability>();
    private bool isRotating;
    private float targetRotation;
    private float rotationSpeed = 90f;
    private Rigidbody2D rb;
    private float originalRotation;
    private void Start() {
        rb=GetComponentInParent<Rigidbody2D>();
        isRotating = false;
        AllAbilities.Add("Swing", new Ability("Swing", Swing, this));
        originalRotation = rb.rotation;
    }

    private void Swing(){
        if (!isRotating)
        {
            isRotating = true;
            originalRotation = rb.rotation;
            targetRotation = rb.rotation - 90f; 
        }
    }
    private void Update()
    {
        if (isRotating)
        {
  
            float step = rotationSpeed * Time.deltaTime;
            float currentRotation = Mathf.MoveTowards(rb.rotation, targetRotation, step);
            rb.MoveRotation(currentRotation);

            if (Mathf.Approximately(rb.rotation, targetRotation))
            {
                isRotating = false;
            }
        }
        else
        {
            float step = rotationSpeed * Time.deltaTime;
            float currentRotation = Mathf.MoveTowards(rb.rotation, originalRotation, step);
            rb.MoveRotation(currentRotation);
        }
    }
}
