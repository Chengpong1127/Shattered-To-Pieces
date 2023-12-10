using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using AbilitySystem;
using AttributeSystem.Components;
using AttributeSystem.Authoring;
using AbilitySystem.Authoring;


public static class GameEvents{

    public static class AssemblyRoomEvents{
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
        /// Server Event.
        /// Triggered after the ability is set to an entry. 
        /// </summary>
        public static Action<AbilityManager, GameComponentAbility, int> OnSetAbilityToEntry = delegate { };
        /// <summary>
        /// Server Event.
        /// Triggered after the ability is set to out of an entry. 
        /// </summary>
        public static Action<AbilityManager, GameComponentAbility> OnSetAbilityOutOfEntry = delegate { };
        public static Action<AbilityManager> OnAbilityManagerUpdated = delegate { };
        /// <summary>
        /// Server Event.
        /// Triggered after setting binding. 
        /// </summary>
        public static Action<int, string> OnSetBinding = delegate { };
    }

    public static class AbilityRunnerEvents{
        /// <summary>
        /// Client Event.
        ///	Triggered after the ability button is pressed.
        /// </summary>
        public static Action<int> OnLocalInputStartAbility = delegate { };
        /// <summary>
        /// Client Event.
        ///	Triggered after the ability button is released.
        /// </summary>
        public static Action<int> OnLocalInputCancelAbility = delegate { };
    }

    public static class RebindEvents{
        /// <summary>
        /// Client Event.
        /// </summary>
        public static Action<int> OnStartRebinding = delegate { };
        /// <summary>
        /// Client Event.
        /// Triggered after a key is rebind.
        /// Parameters: (index, path)
        /// </summary>
        public static Action<int, string> OnFinishRebinding = delegate { };
        public static Action OnCancelRebinding = delegate { };
    }

    public static class GameEffectManagerEvents{
        /// <summary>
        /// Used to request giving a game effect to an entity.
        /// </summary>
        public static Action<Entity, Entity, GameplayEffectScriptableObject> RequestGiveGameEffect = delegate { };
        /// <summary>
        /// Used to request simply modifying an attribute of an entity.
        /// </summary>
        public static Action<Entity, Entity, GameplayEffectModifier> RequestModifyAttribute = delegate { };


    }
    public static class LobbyEvents{
        /// <summary>
        /// Triggered after the lobby data is changed.
        /// </summary>
        public static Action<Dictionary<string, string>> OnLobbyDataChanged = delegate { };
    }

    public static class AttributeEvents{
        /// <summary>
        /// Triggered after the attribute is changed.
        /// Parameters: (entity, attribute, old value, new value)
        /// </summary>
        public static Action<Entity, AttributeScriptableObject, float, float> OnEntityAttributeChanged = delegate { };

        public static Action<Entity, float, float> OnEntityHealthChanged = delegate { };
    }
    public static class GameComponentEvents{
        /// <summary>
        /// Server Event.
        /// Triggered after the game component is selected.
        /// Parameters: (game component, is selected)
        /// </summary>
        public static Action<GameComponent, bool> OnGameComponentSelected = delegate { };
        /// <summary>
        /// Server Event.
        /// Triggered after the game component is connected.
        /// Parameters: (game component, component connected to)
        /// </summary>
        public static Action<GameComponent, GameComponent> OnGameComponentConnected = delegate { };
        /// <summary>
        /// Server Event.
        /// Triggered after the game component is disconnected.
        /// Parameters: (game component, component disconnected from)
        /// </summary>
        public static Action<GameComponent, GameComponent> OnGameComponentDisconnected = delegate { };
        /// <summary>
        /// Server Event.
        /// </summary>
        public static Action<BaseEntity> OnEntityDied = delegate { };
    }

    public static class UIEvents{
        /// <summary>
        /// Client Event.
        /// Triggered after a GameComponentAbility is selected.
        /// Parameters: (ability owner)
        /// </summary>
        public static Action<GameComponent> OnGameComponentAbilitySelected = delegate { };
        /// <summary>
        /// Client Event.
        /// </summary>
        public static Action<GameComponent> OnGameComponentAbilitySelectedEnd = delegate { };
    }
    public static Action<PlayerProfile> OnPlayerProfileUpdated = delegate { };
    public static Action<GameSetting> OnSettingsUpdated = delegate { };
    public static Action<GameRecord> OnGameRecordUpdated = delegate { };
}