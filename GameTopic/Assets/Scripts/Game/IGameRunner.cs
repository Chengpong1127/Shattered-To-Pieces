

using System;
/// <summary>
/// The game runner is the main entry point for the game. It is responsible for starting the game, loading the game, and ending the game.
/// A game runner should be executed by a host or a server.
/// </summary>
public interface IGameRunner{
    public event Action OnGameStarted;
    public event Action OnGameEnded;
    public void StartGame();
}