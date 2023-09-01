using UnityEngine;
using AbilitySystem.Authoring;
using AbilitySystem;
using System.Collections;

[CreateAssetMenu(fileName = "SummonAbility", menuName = "Ability/SummonAbility")]
public class SummonAbility : AbstractAbilityScriptableObject
{
    [SerializeField]
    protected GameObject SummonPrefab;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new SummonAbilitySpec(this, owner)
        {
            SummonPrefab = SummonPrefab
        };
        return spec;
    }
    public class SummonAbilitySpec: RunnerAbilitySpec{
        public GameObject SummonPrefab;
        private ISummonable Summonable;
        public SummonAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            Summonable = SelfEntity as ISummonable ?? throw new System.ArgumentNullException("The entity should implement ISummonable");
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
            var instance = Instantiate(SummonPrefab);
            Summonable.InitSummonObject(instance);
            yield return null;
        }

        protected override IEnumerator PreActivate()
        {
            yield return null;
        }
    }
}