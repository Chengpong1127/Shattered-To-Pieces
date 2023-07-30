using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities { get; private set; } = new Dictionary<string, Ability>();
    public Transform connectAnchor;
    private bool isRotating;
    private bool SkillTriggered = false;
    private float targetRotation=0f;
    private float rotationSpeed = 500f;
    private Rigidbody2D rb;
    private float originalRotation;
    private bool clockwise;
    private void Start() {
        rb=GetComponentInParent<Rigidbody2D>();
        isRotating = false;
        AllAbilities.Add("SwingRight", new Ability("SwingRight", SwingRight, this));
        AllAbilities.Add("SwingLeft", new Ability("SwingLeft", SwingLeft, this));
        originalRotation = rb.rotation;
    }

    private void SwingRight(){
        if (!isRotating)
        {
            isRotating = true;
            clockwise =false;
            targetRotation = 270f;
            SkillTriggered=true;
        }
    }
    private void SwingLeft()
    {
        if (!isRotating)
        {
            isRotating = true;
            clockwise = true;
            targetRotation = 90f;
            SkillTriggered = true;
        }
    }
    private void Update()
    {
        if (isRotating&&SkillTriggered)
        {
            float step = (clockwise ? 1f : -1f)*rotationSpeed * Time.deltaTime;
            float currentRotation = transform.eulerAngles.z;
            float target = targetRotation - currentRotation;
            if (Mathf.Abs(target) > 2f)
            {
                transform.RotateAround(connectAnchor.position, Vector3.forward, step);
            }
            else
            {
                targetRotation = 0f;
                isRotating = false;
            }
        }
        else if(SkillTriggered)
        {
            float currentRotation = transform.eulerAngles.z;
            Debug.Log(currentRotation);
            if (Mathf.Abs(currentRotation) > 2f)
            {
                float remainingRotation = targetRotation - currentRotation;
                float step = (clockwise ? -1f : 1f) * Mathf.Min(Mathf.Abs(remainingRotation), rotationSpeed * Time.deltaTime);
                transform.RotateAround(connectAnchor.position, Vector3.forward, step);
            }
            else
            {
                SkillTriggered = false;
            }
            
        }
    }
}
