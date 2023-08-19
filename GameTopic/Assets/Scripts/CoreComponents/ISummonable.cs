using UnityEngine;


public interface ISummonable{
    /// <summary>
    /// This method is called after the summon object is instantiated. Initialize the summon object here.
    /// </summary>
    public void InitSummonObject(GameObject _object);
}