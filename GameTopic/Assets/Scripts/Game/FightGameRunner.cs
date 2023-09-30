
public class FightGameRunner: BaseGameRunner{
    protected override void PlayerDiedHandler(BasePlayer player)
    {
        base.PlayerDiedHandler(player);
        PlayerExitGame(player);
    }
}