using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wheels : MonoBehaviour, ICoreComponent
{
    public int LocalComponentID { get; set; }
    public int ComponentGUID { get; set; }
    public IConnector Connector { get; }

    public ICoreComponent CoreComponent => this;
    // Implement other IGameComponent interface methods...

    public Dictionary<string, Ability> AllAbilities { get; private set; }
    private Rigidbody2D rb;
    private const float MoveForce = 10f;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        AllAbilities = new Dictionary<string, Ability>
        {
            { "turnRight", new Ability("turnRight", TurnRight) },
            { "turnLeft", new Ability("turnLeft", TurnLeft) }
        };
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            TurnLeft();
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            TurnRight();
        }
    }

    private void TurnRight()
    {
        rb.AddForce(Vector2.right * MoveForce);
    }

    private void TurnLeft()
    {
        rb.AddForce(Vector2.left * MoveForce);
    }
}

