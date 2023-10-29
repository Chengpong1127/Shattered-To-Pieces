using JetBrains.Annotations;
using Unity.Netcode;

public abstract class BaseGameEventHandler: NetworkBehaviour{
    public RunningMode HandlerRunningMode = RunningMode.OnlyServer;
    public enum RunningMode{
        OnlyServer,
        OnlyClient,
        Both
    }
}
