using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{

    public static PlayerControls Instance;

    public event EventHandler OnAttachCableAction;
    public event EventHandler OnGamePauseAction;

    bool isBackwardsThrusting;
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

    public void OnBackwardsThrust(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isBackwardsThrusting = true;
        }
        else if (context.canceled)
        {
            isBackwardsThrusting = false;
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

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnGamePauseAction?.Invoke(this, EventArgs.Empty);
        }

    }

    public bool IsThrusting()
    {
        return isThrusting;
    }

    public bool IsBackwardsThrusting()
    {
        return isBackwardsThrusting;
    }

    public float GetRotationInput()
    {
        return rotationInput;
    }

}