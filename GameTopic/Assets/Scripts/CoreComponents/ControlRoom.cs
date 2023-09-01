using System.Collections.Generic;
using UnityEngine;

public class ControlRoom : BaseCoreComponent, ICharacter
{
    [field:SerializeField] public CharacterController Character { get; private set; }
    [SerializeField] Vector3 GravityMove;

    private void FixedUpdate() {
        // if (!Character.isGrounded) Character.Move(GravityMove * Time.fixedDeltaTime);
    }
}