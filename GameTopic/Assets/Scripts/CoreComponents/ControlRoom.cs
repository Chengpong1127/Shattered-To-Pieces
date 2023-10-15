using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Unity.Netcode;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTagNamespace.Authoring;

public class ControlRoom : BaseCoreComponent, IDeviceRoot {
    public Device Device { get; set; }
}