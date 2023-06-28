using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssemblySystemManager : MonoBehaviour
{
    IDevice ControlledDevice;
    IGameComponentFactory GameComponentFactory;
    ComponentMover componentMover;
    public Dictionary<Guid, IGameComponent> GlobalComponentMap {get; private set; } = new Dictionary<Guid, IGameComponent>();

    private void Start() {
        componentMover = gameObject.AddComponent<ComponentMover>();
        componentMover.inputManager = new InputManager();
        
    }
}
