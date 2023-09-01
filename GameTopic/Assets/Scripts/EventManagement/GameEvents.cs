using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameEvents{

    public static class AssemblyRoomEvents{
        /// <summary>
        /// Event for when the room mode is changed. 
        /// </summary>
        public static Action<AssemblyRoomMode> OnSetRoomMode = delegate { };

        /// <summary>
        /// Called when the device is loaded. 
        /// </summary>
        public static Action OnLoadedDevice = delegate { };

        /// <summary>
        /// Called when the device is saved. 
        /// </summary>
        public static Action OnSavedDevice = delegate { };
    }
    public static class AbilityManagerEvents{
        /// <summary>
        /// Triggered after the ability is set to an entry. 
        /// </summary>
        public static Action<GameComponentAbility> OnSetAbilityToEntry = delegate { };
        /// <summary>
        /// Triggered after the ability is set to out of an entry. 
        /// </summary>
        public static Action<GameComponentAbility> OnSetAbilityOutOfEntry = delegate { };
        /// <summary>
        /// Triggered after setting binding. 
        /// </summary>
        public static Action<int, string> OnSetBinding = delegate { };
    }

    public static class AbilityRunnerEvents{
        /// <summary>
        ///	Triggered after the ability button is pressed.
        /// </summary>
        public static Action<int> OnLocalStartAbility = delegate { };
        /// <summary>
        ///	Triggered after the ability button is released.
        /// </summary>
        public static Action<int> OnLocalCancelAbility = delegate { };
    }

    public static class AssemblySystemManagerEvents{
        /// <summary>
        /// Triggered after the game component is dragged. 
        /// </summary>
        public static Action<IGameComponent> OnGameComponentDraggedStart = delegate { };
        /// <summary>
        /// Triggered after the game component is dragged. 
        /// </summary>
        public static Action<IGameComponent> OnGameComponentDraggedEnd = delegate { };
        /// <summary>
        /// Triggered after the game component is connected. 
        /// </summary>
        public static Action<IGameComponent> AfterGameComponentConnected = delegate { };
    }

    public static class RebindEvents{
        /// <summary>
        /// Triggered after a key is rebind.
        /// Parameters: (index, path)
        /// </summary>
        public static Action<int, string> OnFinishRebinding = delegate { };
    }

    public static class GameEffectManagerEvents{
        /// <summary>
        /// Used to request giving a game effect to an entity.
        /// </summary>
        public static Action<Entity, Entity, AbilitySystem.Authoring.GameplayEffectScriptableObject> RequestGiveGameEffect = delegate { };
        /// <summary>
        /// Used to request simply modifying an attribute of an entity.
        /// </summary>
        public static Action<Entity, Entity, AbilitySystem.GameplayEffectModifier> RequestModifyAttribute = delegate { };
    }
    public static class LobbyEvents{
        /// <summary>
        /// Triggered after the lobby data is changed.
        /// </summary>
        public static Action<System.Collections.Generic.Dictionary<string, string>> OnLobbyDataChanged = delegate { };
    }

}