using System;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
public class FightGameRunner: GameRunner{
    protected override void PlayerDiedHandler(BasePlayer player)
    {
        Debug.Log("FightGameRunner.PlayerDiedHandler");
        base.PlayerDiedHandler(player);
        GameOver(GetGameResult(player));
    }

    protected GameResult GetGameResult(BasePlayer diedPlayer){
        GameResult result = new GameResult();
        result.PlayerRankMap[diedPlayer.OwnerClientId] = 2;
        PlayerMap.Values.ToList().ForEach(player => {
            if (player != diedPlayer){
                result.PlayerRankMap[player.OwnerClientId] = 1;
            }
        });
        return result;
    }
}