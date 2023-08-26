using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using DG.Tweening;
[CreateAssetMenu(fileName = "InvisibleAbility", menuName = "Ability/InvisibleAbility")]
public class InvisibleAbility : AbstractAbilityScriptableObject
{
    [SerializeField]
    Material invisible;
    [SerializeField]
    Material m_default;
    protected float DurationTime=4.0f;

    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new InvisibleAbilitySpec(this, owner)
        {
            m_default = m_default,
            invisible = invisible,
            DurationTime=DurationTime
        };

        return spec;

    }
    protected class InvisibleAbilitySpec : EntityAbilitySpec
    {
        public Material m_default;
        public Material invisible;
        public BaseCoreComponent ControlRoom;
        public float DurationTime;
        public BaseCoreComponent[] baseCoreComponents;
        public InvisibleAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {

            
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
            var root = SelfEntity.transform.root;
            while (root.parent != null)
            {
                Debug.Log(root.name);
                root = root.transform.root;
            }
            //Debug.Log(SelfEntity.transform.root.name);
            ControlRoom = root.GetChild(0).GetComponent<BaseCoreComponent>();
            baseCoreComponents = ControlRoom.GetAllChildren();
            foreach (var baseComponent in baseCoreComponents)
            {
                SpriteRenderer s = baseComponent.GetComponent<SpriteRenderer>();
                s.material = invisible;
            }
            yield return new WaitForSeconds(DurationTime);
            foreach (var baseComponent in baseCoreComponents)
            {
                SpriteRenderer s = baseComponent.GetComponent<SpriteRenderer>();
                s.material = m_default;
            }
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}

