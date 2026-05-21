using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{

    public static PlayerControls Instance;

    public event EventHandler OnAttachCableAction;

    bool isThrusting;
    float rotationInput;

    void Awake()
    {
        Instance = this;
    }

    public void OnThrust(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isThrusting = true;
        }
        else if (context.canceled)
        {
            isThrusting = false;
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        rotationInput = context.ReadValue<float>();
    }

    public void OnAttachCable(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnAttachCableAction?.Invoke(this, EventArgs.Empty);
        }

    }

    public bool IsThrusting()
    {
        return isThrusting;
    }

    public float GetRotationInput()
    {
        return rotationInput;
    }

}