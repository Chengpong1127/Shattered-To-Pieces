using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameComponentFactory{
    IGameComponent CreateGameComponentObject(int id);
}
