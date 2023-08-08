using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowOutAffect : SkillAffectBase {
    List<Vector3> startPosition { get;set; } = new List<Vector3>();
    List<Quaternion> startRotation { get; set; } = new List<Quaternion>();

    [field: SerializeField] float throwPower { get; set; } = 100f;

    List<Collider2D> resultList { get; set; } = new List<Collider2D> ();
    ContactFilter2D filter { get;set; } = new ContactFilter2D ();

    public ThrowOutAffect() {
        this.type = SkillAffectType.Other;
    }

    override public void Invoke() {
        int loopIndex = 0;
        while (loopIndex < affectedObjectList.Count) {
            affectedObjectList[loopIndex].collider.OverlapCollider(filter,resultList);
            resultList.ForEach(collider => {
               //  Debug.Log(collider.name + "  " + collider.transform.parent.name);
                BaseCoreComponent hitedCore = collider.GetComponent<BaseCoreComponent>();
                if(!(hitedCore != null && hitedCore.HasTheSameRootWith(owner.coreComponent))) {
                    OnHitObject();
                    SetToDefault();
                }
            });

            loopIndex++;
        }
    }

    public void InvokeStart() {
        if (execute) { return; }
        interrupt = false;
        execute = true;
        int loopIndex = 0;
        while(loopIndex < affectedObjectList.Count) {
            if (! (affectedObjectList[loopIndex].IsRigidbodyAffected &&
                affectedObjectList[loopIndex].IsTransformAffected &&
                affectedObjectList[loopIndex].IsColliderAffected &&
                affectedObjectList[loopIndex].IsJointAffected)) { affectedObjectList.RemoveAt(loopIndex); continue; }

            startPosition.Add(affectedObjectList[loopIndex].transform.position);
            startRotation.Add(affectedObjectList[loopIndex].transform.rotation);

            affectedObjectList[loopIndex].joint.enabled = false;
            affectedObjectList[loopIndex].rigidbody.AddForce(affectedObjectList[loopIndex].transform.right * throwPower,ForceMode2D.Force);

            loopIndex++;
        }
    }
    public void OnHitObject() {
        Debug.Log("Boooo.");
    }
    public void SetToDefault() {
        execute = false;
        int loopIndex = 0;
        while (loopIndex < affectedObjectList.Count) {
            affectedObjectList[loopIndex++].joint.enabled = true;
        }
    }

    public IEnumerator FrameRunner() {
        while (execute)
        {
            Invoke();
            yield return new WaitForFixedUpdate();
        }
    }
}
