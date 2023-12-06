
public class GameSetting
{
    public float MasterVolume;
    public float MusicVolume;
    public float SoundVolume;

    public static GameSetting DefaultSetting()
    {
        return new GameSetting()
        {
            MasterVolume = 1,
            MusicVolume = 1,
            SoundVolume = 1
        };
    }

}