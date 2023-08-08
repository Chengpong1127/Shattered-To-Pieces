using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderAffect : SkillAffectBase {
    float rotateSpeedMultiplier { get; set; } = -360.0f;
    float rotateAngle { get; set; } = -90.0f;
    float startAngle { get; set; } // 0 ~ 360
    float endingAngle { get; set; } // 0 ~ 360
    public Transform transform { get; set; } = null;
    public Transform colliderTransform { get; set; } = null;

    public LoaderAffect() {
        this.type = SkillAffectType.Attack;
    }

    override public void Invoke() {
        if (!transform || !colliderTransform) { return; }
        transform?.Rotate(0,0, rotateSpeedMultiplier * Time.fixedDeltaTime, Space.Self);
        colliderTransform?.Rotate(0,0, rotateSpeedMultiplier * Time.fixedDeltaTime, Space.Self);
    }
    public IEnumerator FrameRunner() {
        while (execute && !interrupt &&
            (transform.localRotation.eulerAngles.z + 90) % 360 > endingAngle && (transform.localRotation.eulerAngles.z + 90) % 360 <= startAngle) {
            Invoke();
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        SetToDefault();
    }

    public void InvokeStart() {
        if(execute || !transform || !colliderTransform) { return; }
        execute = true;
        interrupt = false;
        startAngle = transform.localRotation.eulerAngles.z + 90;
        endingAngle = (startAngle + rotateAngle) % 360;
    }

    public void SetToDefault() {
        execute = false;
        interrupt = false;
        if (!transform || !colliderTransform) { return; }
        transform.localRotation = Quaternion.Euler(0, 0, startAngle - 90);
        colliderTransform.localRotation = Quaternion.Euler(0, 0, startAngle - 90);
    }
}
