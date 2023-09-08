using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Rubbergun :BaseCoreComponent,IAimable, IBodyControlable
{
    [SerializeField]
    public GameObject bullet;
    [SerializeField]
    public Transform ShootPoint;
    public Vector2 AimStartPoint => BodyTransform.position;
    public BaseCoreComponent body { get; private set; }

    protected override void Awake()
    {
        body = this;
        base.Awake();
    }
    public void EndAim(Vector2 aimVector)
    {
        var b=Instantiate(bullet,new Vector3(ShootPoint.position.x, ShootPoint.position.y, 0),Quaternion.identity);
        b.GetComponent<NetworkObject>()?.Spawn();
        b.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        b.GetComponent<Rigidbody2D>().velocity = aimVector * 0.05f;
    }

    public void StartAim(Vector2 aimVector)
    {
    }

}
