using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowOutAffect : SkillAffectBase {
    List<Vector3> startPosition { get;set; } = new List<Vector3>();
    List<Quaternion> startRotation { get; set; } = new List<Quaternion>();

    [field: SerializeField] float throwPower { get; set; } = 3f;

    public ThrowOutAffect() {
        this.type = SkillAffectType.Other;
    }

    // override public void Invoke() {
    //     int loopIndex = 0;
    //     while (loopIndex < affectedObjectList.Count) {
    //         loopIndex++;    
    //     }
    // }

    public void InvokeStart() {
        if (execute) { return; }
        interrupt = false;
        execute = true;
        int loopIndex = 0;
        while(loopIndex < affectedObjectList.Count) {
            if (! (affectedObjectList[loopIndex].IsRigidbodyAffected &&
                affectedObjectList[loopIndex].IsTransformAffected &&
                affectedObjectList[loopIndex].IsColliderAffected)) { affectedObjectList.RemoveAt(loopIndex); continue; }

            startPosition.Add(affectedObjectList[loopIndex].transform.position);
            startRotation.Add(affectedObjectList[loopIndex].transform.rotation);

            affectedObjectList[loopIndex].rigidbody.AddForce(affectedObjectList[loopIndex].transform.right * throwPower,ForceMode2D.Force);

            loopIndex++;
        }
    }
    public void SetToDefault() {

    }
}
