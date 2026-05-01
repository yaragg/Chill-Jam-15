using System.Collections;
using deVoid.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : Manager<InputManager>
{
    private PlayerInput _playerInput;
    private PlayerControls _controls;
    private bool _isDragging = false;
    private Vector2 _dragStartPosition;

    protected override IEnumerator Initialize()
    {
        _controls = new PlayerControls();
        _controls.Gameplay.Enable();
        _controls.Gameplay.Click.started += HandleClickStarted;
        _controls.Gameplay.Click.canceled += HandleClickCanceled;
        _controls.Gameplay.Click.performed += HandleClickPerformed;
        _controls.Gameplay.RightClick.performed += HandleRightClickPerformed;

        return base.Initialize();
    }

    private void HandleClickStarted (InputAction.CallbackContext context)
    {
        Utils.LogMessage(this, $"click started {context.interaction.GetType().Name}");
        _dragStartPosition = GetMouseWorldPosition();
    }

    private void HandleClickPerformed (InputAction.CallbackContext context)
    {
        Utils.LogMessage(this, $"click performed {context.interaction.GetType().Name}");
        Vector2 worldPosition = GetMouseWorldPosition();

        if (context.interaction is TapInteraction)
        {
            Signals.Get<TapSignal>().Dispatch(worldPosition);
        }
        else if (context.interaction is HoldInteraction)
        {
            _isDragging = true;
            Signals.Get<DragSignal>().Dispatch(_dragStartPosition, true);
        }
    }

    private void HandleClickCanceled (InputAction.CallbackContext context)
    {
        Utils.LogMessage(this, $"click canceled {context.interaction.GetType().Name}");
        if (_isDragging && context.interaction is HoldInteraction)
        {
            _isDragging = false;
            
            Signals.Get<DragSignal>().Dispatch(GetMouseWorldPosition(), false);
        }
    }

    private void HandleRightClickPerformed (InputAction.CallbackContext context)
    {
        Utils.LogMessage(this, $"rotate performed");
        Signals.Get<RotateSignal>().Dispatch(GetMouseWorldPosition());
    }

    public Vector2 GetMouseWorldPosition ()
    {
        Vector2 mousePosition = _controls.Gameplay.Point.ReadValue<Vector2>();
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return worldPosition;
    }
}