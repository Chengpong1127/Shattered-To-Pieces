


public static class EventName
{
	/// <summary>
	/// Events in AssemblyRoom
	/// </summary>
	public static class AssemblyRoomEvents
	{
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

	public static class AbilityManagerEvents
	{
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

	public static class BuffEvents {
		public static string AddBuff = nameof(BuffEvents) + nameof(AddBuff);
		public static string RemoveBuff = nameof(BuffEvents) + nameof(RemoveBuff);
        public static string OnTrigger(Entity target, string eventName) { return target.GetHashCode() + eventName; }
    }

	public static class DraggableMoverEvents
	{
		/// <summary>
		/// Triggered after the draggable is dragged. 
		/// Handler Type: Action<IDraggable, Vector2>
		/// </summary>
		public const string OnDragStart = nameof(DraggableMoverEvents) + nameof(OnDragStart);
		/// <summary>
		/// Triggered after the draggable is dragged. 
		/// Handler Type: Action<IDraggable, Vector2>
		/// </summary>
		public const string OnDragEnd = nameof(DraggableMoverEvents) + nameof(OnDragEnd);
		/// <summary>
		/// Triggered after the draggable is dragged. 
		/// Handler Type: Action<IDraggable, Vector2>
		/// </summary>
		public const string OnScrollWhenDragging = nameof(DraggableMoverEvents) + nameof(OnScrollWhenDragging);
	}
	public static class AssemblySystemManagerEvents
	{
		/// <summary>
		/// Triggered after the game component is dragged. 
		/// Handler Type: Action<IGameComponent>
		/// </summary>
		public const string OnGameComponentDraggedStart = nameof(AssemblySystemManagerEvents) + nameof(OnGameComponentDraggedStart);
		/// <summary>
		/// Triggered after the game component is dragged. 
		/// Handler Type: Action<IGameComponent>
		/// </summary>
		public const string OnGameComponentDraggedEnd = nameof(AssemblySystemManagerEvents) + nameof(OnGameComponentDraggedEnd);
		/// <summary>
		/// Triggered after the game component is connected. 
		/// Handler Type: Action<IGameComponent>
		/// </summary>
		public const string AfterGameComponentConnected = nameof(AssemblySystemManagerEvents) + nameof(AfterGameComponentConnected);
	}
}