using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenGun : BaseCoreComponent, ISummonable {
    [SerializeField] public Transform SpwanPoint;
    [SerializeField] public float Power;

    public void InitSummonObject(GameObject _object) {
        if( _object == null) { return; }

        var summon = Instantiate(_object, SpwanPoint.position, SpwanPoint.rotation);
        var summonObj = summon.GetComponentInChildren<BaseCoreComponent>() ?? throw new System.ArgumentNullException("This object should have BaseCoreComponent.");
        var summonSetting = (summonObj as ICreated) ?? throw new System.ArgumentNullException("The entity should have ICreated interface.");
        summonSetting.Owner = this;

        summonObj.BodyRigidbody.AddForce(
            SpwanPoint.TransformDirection(Vector3.right) * Power,
            ForceMode2D.Impulse
        );
    }
}
