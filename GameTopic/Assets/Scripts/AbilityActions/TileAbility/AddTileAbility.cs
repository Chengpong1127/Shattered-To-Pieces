using AbilitySystem;
using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "AddTileAbility", menuName = "Ability/AddTileAbility")]
public class AddTileAbility : DisplayableAbilityScriptableObject
{
    [SerializeField]
    public string TileName;
    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        return new AddTileAbilitySpec(this, owner)
        {
            TileName = TileName
        };
    }

    public class AddTileAbilitySpec : RunnerAbilitySpec
    {
        public string TileName;
        public AddTileAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
        }

        public override void CancelAbility()
        {
            
        }
        public override bool CanActivateAbility()
        {
            var targetPoints = (SelfEntity as ITileTargeter).GetTargetTileCoordinates();
            var mapManager = GameRunner.ServerGameRunnerInstance.MapManager;
            return base.CanActivateAbility() && targetPoints.Any(point => !mapManager.CurrentTileMap.HasTile((Vector3Int)point));
        }

        protected override IEnumerator ActivateAbility()
        {
            var targetPoints = (SelfEntity as ITileTargeter).GetTargetTileCoordinates();
            targetPoints.ToList().ForEach(point => GameRunner.ServerGameRunnerInstance.MapManager.SetTile(point, TileName, false));
            yield return null;
        }
    }
}
