using System;
using System.Linq;
using Cysharp.Threading.Tasks;
public class FightGameRunner: BaseGameRunner{
    protected override void PlayerDiedHandler(BasePlayer player)
    {
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