using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BatAffect", menuName = "SkillAffect/BatAffect")]
public class BatAffect : SkillAffectBase
{
    bool isRotating { get; set; }=false;
    bool SkillTriggered { get; set; } = false;
    float targetRotation { get; set; } = 0f;
    float oringnalRotation { get; set; }=0f;
    float rotationSpeed { get; set; } = 500f;
    public bool clockwise { get; set; }
    Transform transform { get; set; } = null;
    public Transform connectAnchor { get; set; }= null;
    public BatAffect()
    {
        this.type = SkillAffectType.Attack;
    }

    override public void Invoke()
    {
        transform= affectedObjectList.Count == 1 && affectedObjectList[0].IsTransformAffected ? affectedObjectList[0].transform : null;
        if (transform == null) return;
        if (!isRotating && !SkillTriggered)
        {
            isRotating = true;
            targetRotation = transform.rotation.eulerAngles.z - (clockwise ? -90f : 90f);
            if (targetRotation < 0f) targetRotation += 360f;
            targetRotation %= 360f;
            oringnalRotation = transform.rotation.eulerAngles.z;
            if (oringnalRotation < 0f) oringnalRotation += 360f;
            Debug.Log(oringnalRotation + " " + targetRotation);
            SkillTriggered = true;
        }
    }
   
    public IEnumerator RotateCoroutine()
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
