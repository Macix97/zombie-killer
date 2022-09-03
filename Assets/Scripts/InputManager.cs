using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    private Vector2 _moveInput;
    private Vector2 _cursorPosition;
    private PlayerInput _playerInput;

    public static event Action OnFire;
    public static event Action OnPauseToggle;

    public static Vector2 MoveInput => _instance ? _instance._moveInput : Vector2.zero;
    public static Vector2 CursorPosition => _instance ? _instance._cursorPosition : Vector2.zero;

    private void Awake()
    {
        _instance = this;
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        Menu.OnPauseToggle += SwitchPlayerActions;
        SceneLoader.OnLoadingStarted += DisableUIInputModule;
        PlayerCharacter.OnPlayerDead += DisableAllPlayerActions;
    }

    private void OnDisable()
    {
        Menu.OnPauseToggle -= SwitchPlayerActions;
        SceneLoader.OnLoadingStarted -= DisableUIInputModule;
        PlayerCharacter.OnPlayerDead -= DisableAllPlayerActions;
    }

    public void OnUILeftClickAction(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Canceled) DeselectCurrent();
    }

    public void OnMoveAction(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void OnCursorPositionAction(InputAction.CallbackContext context)
    {
        _cursorPosition = context.ReadValue<Vector2>();
    }

    public void OnFireAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled) OnFire?.Invoke();
    }

    public void OnPauseToggleAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Canceled) OnPauseToggle?.Invoke();
    }

    private void DeselectCurrent()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null);
    }

    private void DisableUIInputModule(SceneLoader.SceneName sceneName)
    {
        _playerInput.uiInputModule.enabled = false;
    }

    private void DisableAllPlayerActions()
    {
        _playerInput.currentActionMap.Disable();
    }

    private void SwitchPlayerActions(bool isPause)
    {
        foreach (InputAction action in _instance._playerInput.currentActionMap.actions)
        {
            if (action.name == ActionName.TogglePause.ToString()) continue;
            if (isPause) action.Disable();
            else action.Enable();
        }
    }

    private enum ActionName
    {
        Move,
        CursorPosition,
        Fire,
        TogglePause,
    }
}
