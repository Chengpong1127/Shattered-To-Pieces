using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities { get; private set; } = new Dictionary<string, Ability>();
    public Transform connectAnchor;
    private bool isRotating;
    private bool SkillTriggered = false;
    private float targetRotation = 0f;
    private float oringnalRotation;
    private float rotationSpeed = 500f;
    private Rigidbody2D rb;
    private bool clockwise;
    public IGameComponent OwnerGameComponent { get; set; }
    private void Awake() {
        AllAbilities.Add("SwingRight", new Ability("SwingRight", SwingRight, this));
        AllAbilities.Add("SwingLeft", new Ability("SwingLeft", SwingLeft, this));
    }
    private void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        isRotating = false;
        
    }

    private void SwingRight()
    {
        if (!isRotating && !SkillTriggered)
        {
            isRotating = true;
            clockwise = false;
            targetRotation = transform.rotation.eulerAngles.z - 90f;
            if (targetRotation < 0f) targetRotation += 360f;
            oringnalRotation = transform.rotation.eulerAngles.z;
            SkillTriggered = true;
            StartCoroutine(RotateCoroutine());
        }
    }

    private void SwingLeft()
    {
        if (!isRotating && !SkillTriggered)
        {
            isRotating = true;
            clockwise = true;
            targetRotation = transform.rotation.eulerAngles.z + 90f;
            if (targetRotation > 360f) targetRotation -= 360f;
            oringnalRotation = transform.rotation.eulerAngles.z;
            SkillTriggered = true;
            StartCoroutine(RotateCoroutine());
        }
    }

    private IEnumerator RotateCoroutine()
    {
        while (isRotating && SkillTriggered)
        {
            float currentRotation = transform.rotation.eulerAngles.z;
            float target = targetRotation - currentRotation;
            if (target < -360f) target += 360f;
            if (target > 360f) target -= 360f;
            if (Mathf.Abs(target) > 2f)
            {
                float step = (clockwise ? 1f : -1f) * Mathf.Min(Mathf.Abs(target), rotationSpeed * Time.fixedDeltaTime);
                transform.RotateAround(connectAnchor.position, Vector3.forward, step);
            }
            else
            {
                targetRotation = oringnalRotation;
                isRotating = false;
            }

            yield return new WaitForFixedUpdate();
        }

        while (SkillTriggered)
        {
            float currentRotation = transform.rotation.eulerAngles.z;
            float target = targetRotation - currentRotation;
            if (target < -360f) target += 360f;
            if (target > 360f) target -= 360f;
            if (Mathf.Abs(target) > 2f)
            {
                float step = (clockwise ? -1f : 1f) * Mathf.Min(Mathf.Abs(target), rotationSpeed * Time.fixedDeltaTime);
                transform.RotateAround(connectAnchor.position, Vector3.forward, step);
            }
            else
            {
                SkillTriggered = false;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
