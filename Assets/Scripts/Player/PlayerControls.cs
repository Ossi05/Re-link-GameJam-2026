using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{

    public static PlayerControls Instance;
    public Vector2 MoveInput { get; private set; }


    void Awake()
    {
        Instance = this;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

}