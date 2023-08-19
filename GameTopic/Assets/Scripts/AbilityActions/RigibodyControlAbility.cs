using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
[CreateAssetMenu(fileName = "RigibodyControlAbility", menuName = "Ability/RigibodyControlAbility")]
public class RigibodyControlAbility : AbstractAbilityScriptableObject
{
    [SerializeField]
    private RigidbodyType2D SetRigidbodyType;
    [SerializeField]
    private Vector2 GivenForce;
    [SerializeField]
    private float GivenTorque;
    [SerializeField]
    private ForceMode2D ForceMode;
    [SerializeField]
    private bool SetVelocity = false;
    [SerializeField]
    private Vector2 GivenVelocity;
    [SerializeField]
    private bool SetAngularVelocity = false;
    [SerializeField]
    private float GivenAngularVelocity;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new RigibodyControlAbilitySpec(this, owner)
        {
            SetRigidbodyType = SetRigidbodyType,
            GivenForce = GivenForce,
            ForceMode = ForceMode,
            GivenTorque = GivenTorque,
            GivenVelocity = GivenVelocity,
            GivenAngularVelocity = GivenAngularVelocity,
            SetVelocity = SetVelocity,
            SetAngularVelocity = SetAngularVelocity
        };
        return spec;
    }
    public class RigibodyControlAbilitySpec : EntityAbilitySpec
    {
        private Rigidbody2D Rigidbody2D;
        public RigidbodyType2D SetRigidbodyType;
        public Vector2 GivenForce;
        public float GivenTorque;
        public ForceMode2D ForceMode;
        public Vector2 GivenVelocity;
        public float GivenAngularVelocity;
        public bool SetVelocity;
        public bool SetAngularVelocity;

        public RigibodyControlAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            Rigidbody2D = SelfEntity.BodyRigidbody;
        }
        public override void CancelAbility()
        {
            return;
        }

        public override bool CheckGameplayTags()
        {
            return true;
        }

        protected override IEnumerator ActivateAbility()
        {
            Rigidbody2D.bodyType = SetRigidbodyType;
            Rigidbody2D.AddForce(GivenForce, ForceMode);
            Rigidbody2D.AddTorque(GivenTorque, ForceMode);
            if (SetVelocity)
            {
                Rigidbody2D.velocity = GivenVelocity;
            }
            if (SetAngularVelocity)
            {
                Rigidbody2D.angularVelocity = GivenAngularVelocity;
            }
            EndAbility();
            yield break;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}
