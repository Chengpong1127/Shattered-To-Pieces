using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RotatorAffect", menuName = "SkillAffect/RotatorAffect")]
public class RotatorAffect : SkillAffectBase {
    float rotateSpeedMultiplier { get; set; } = 1.5f;
    float rotateAngle { get; set; } = 90.0f;
    float startAngle { get; set; } // 0 ~ 360
    float endingAngle { get; set; } // 0 ~ 360
    Transform transform { get; set; } = null;
    bool isPostive { get; set; } = true;

    public Transform rotateAnchor { get; set; } = null;
    public RotatorAffect() {
        this.type = SkillAffectType.Move;
    }


    override public void Invoke() {
        if((!execute && interrupt) || !transform) { SetToDefault(); return; }

        if(isPostive) {
            // transform.Rotate(0, 0, rotateAngle * rotateSpeedMultiplier * Time.fixedDeltaTime, Space.Self);
            transform.RotateAround(rotateAnchor.position, Vector3.forward, rotateAngle * rotateSpeedMultiplier * Time.fixedDeltaTime);
            isPostive = transform.localRotation.eulerAngles.z + 180 < endingAngle;
        } else {
            // transform.Rotate(0, 0, -rotateAngle * rotateSpeedMultiplier * Time.fixedDeltaTime, Space.Self);
            transform.RotateAround(rotateAnchor.position, Vector3.forward, -rotateAngle * rotateSpeedMultiplier * Time.fixedDeltaTime);
            isPostive = transform.localRotation.eulerAngles.z + 180 < startAngle;
        }
    }

    public void InvokeStart() {
        isPostive = true;
        interrupt = false;
        transform = affectedObjectList.Count == 1 && affectedObjectList[0].IsTransformAffected ? affectedObjectList[0].transform : null;
        execute = transform && rotateAnchor;
        startAngle = transform.localRotation.eulerAngles.z + 180;
        endingAngle = (startAngle + rotateAngle) % 360;
    }
    public  void SetToDefault() {
        if(!transform) { return; }
        transform.localRotation = Quaternion.Euler(0, 0, startAngle);
    }
}
