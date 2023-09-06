using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerInputManager: MonoBehaviour{
    public PlayerInput playerInput;
    public InputActionMap GameActionMap { get; set;}
    public InputActionMap AbilityActionMap { get; set;}
    public InputAction FlipComponentAction => GameActionMap.FindAction("FlipComponent");
    public InputAction DragComponentAction => GameActionMap.FindAction("Drag");
    public InputAction AssemblyToggleAction => GameActionMap.FindAction("AssemblyToggle");
    public InputAction RegenerateAction => GameActionMap.FindAction("Regenerate");

    public void Awake()
    {
        Debug.Assert(playerInput != null, "playerInput != null");
        GameActionMap = playerInput.actions.FindActionMap("Game");

    }

}