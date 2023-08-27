


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
	public static class AbilityRunningEvents{
		/// <summary>
		///	Triggered after the ability button is pressed.
		/// Handler Type: Action<int> (AbilityIndex)
		/// </summary>
		public const string OnLocalStartAbility = nameof(AbilityManagerEvents) + nameof(OnLocalStartAbility);
		/// <summary>
		///	Triggered after the ability button is released.
		///	Handler Type: Action<int> (AbilityIndex)
		/// </summary>
		public const string OnLocalCancelAbility = nameof(AbilityManagerEvents) + nameof(OnLocalCancelAbility);
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

	public static class AbilityRebinderEvents{
		/// <summary>
		/// Triggered after a key is rebind.
		/// Handler Type: Action<string>
		/// </summary>
		public const string OnFinishRebinding = nameof(AbilityRebinderEvents) + nameof(OnFinishRebinding);
	}

	public static class GameEffectManagerEvents{
		/// <summary>
		/// Used to request giving a game effect to an entity.
		/// Handler Type: Action<Entity, Entity, GameplayEffectScriptableObject> (sender, receiver, gameplayEffect)
		/// </summary>
		public const string RequestGiveGameEffect = nameof(GameEffectManagerEvents) + nameof(RequestGiveGameEffect);
		/// <summary>
		/// Used to request simply modifying an attribute of an entity.
		/// Handler Type: Action<Entity, Entity, GameplayEffectModifier> (sender, receiver, modifier)
		/// </summary>
		public const string RequestModifyAttribute = nameof(GameEffectManagerEvents) + nameof(RequestModifyAttribute);
	}
}