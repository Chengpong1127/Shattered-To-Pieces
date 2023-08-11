


public static class EventName{
    /// <summary>
    /// Events in AssemblyRoom
    /// </summary>
    public static class AssemblyRoomEvents{
        /// <summary>
        /// Event for when the room mode is changed. 
        /// Handler Type: Action<AssemblyRoomMode>
        /// </summary>
        public const string OnSetRoomMode = nameof(AssemblyRoomEvents) + nameof(OnSetRoomMode);

        /// <summary>
        /// Called when the device is loaded. 
        /// Handler Type: Action
        /// </summary>
        public const string OnLoadedDevice = nameof(AssemblyRoomEvents) + nameof(OnLoadedDevice);

        /// <summary>
        /// Called when the device is saved. 
        /// Handler Type: Action
        /// </summary>
        public const string OnSavedDevice = nameof(AssemblyRoomEvents) + nameof(OnSavedDevice);
    }

    public static class AbilityManagerEvents{
        /// <summary>
        /// Triggered after the ability is set to an entry. 
        /// Handler Type: Action<Ability>
        /// </summary>
        public const string OnSetAbilityToEntry = nameof(AbilityManagerEvents) + nameof(OnSetAbilityToEntry);
        /// <summary>
        /// Triggered after the ability is set to out of an entry. 
        /// Handler Type: Action<Ability>
        /// </summary>
        public const string OnSetAbilityOutOfEntry = nameof(AbilityManagerEvents) + nameof(OnSetAbilityOutOfEntry);
        /// <summary>
        /// Triggered after setting binding. 
        /// Handler Type: Action<int, string>
        /// </summary>
        public const string OnSetBinding = nameof(AbilityManagerEvents) + nameof(OnSetBinding);
    }

    public static class BuffAffectedObjectEvents {
        public static string OnEventTrigger(BuffAffectedObject obj, string eventName) { return obj.GetHashCode() + eventName; }
    }
}