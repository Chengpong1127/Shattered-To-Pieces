using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerInputManager: SingletonMonoBehavior<LocalPlayerInputManager>{
    public PlayerInput playerInput;
    public InputActionMap GameActionMap => playerInput.currentActionMap;
    public InputActionMap AbilityActionMap { get; set;}
    public InputAction FlipComponentAction => GameActionMap.FindAction("FlipComponent");
    public InputAction DragComponentAction => GameActionMap.FindAction("Drag");
    public InputAction AssemblyToggleAction => GameActionMap.FindAction("AssemblyToggle");
    public InputAction RegenerateAction => GameActionMap.FindAction("Regenerate");
    public GameObject GetGameObjectUnderMouse(){
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        return GetGameObjectUnderMouse(mousePosition);
    }
    private GameObject GetGameObjectUnderMouse(Vector2 mousePosition)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit;
        hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
    public void Awake()
    {
        Debug.Assert(playerInput != null, "playerInput != null");

    }

}