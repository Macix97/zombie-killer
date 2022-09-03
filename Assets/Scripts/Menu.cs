using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup), typeof(Animator))]
public class Menu : MonoBehaviour
{
    [SerializeField] private Type _type;


    private bool _isPause;
    private Animator _animator;

    public static event Action<bool> OnPauseToggle;

    private void Awake()
    {
        Time.timeScale = 1.0f;
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        switch (_type)
        {
            case Type.Pause:
                InputManager.OnPauseToggle += TogglePause;
                break;
            case Type.GameOver:
                PlayerCharacter.OnPlayerDead += ShowGameOverMenu;
                break;
        }
    }

    private void OnDisable()
    {
        switch (_type)
        {
            case Type.Pause:
                InputManager.OnPauseToggle -= TogglePause;
                break;
            case Type.GameOver:
                PlayerCharacter.OnPlayerDead -= ShowGameOverMenu;
                break;
        }
    }

    private void TogglePause()
    {
        _isPause = !_isPause;
        Time.timeScale = _isPause ? 0.0f : 1.0f;
        SetAnimatorState(_isPause ? Int_State.On : Int_State.Off);
        OnPauseToggle?.Invoke(_isPause);
    }

    private void ShowGameOverMenu()
    {
        StartCoroutine(OnShowingGameOverMenu());
    }

    private IEnumerator OnShowingGameOverMenu()
    {
        yield return new WaitForSeconds(GameSettings.GameOverDelay);
        SetAnimatorState(Int_State.On);
    }

    private void SetAnimatorState(Int_State state)
    {
        _animator.SetInteger(nameof(Int_State), (int)state);
    }

    public void OnStartButtonClicked()
    {
        SceneLoader.Load(SceneLoader.SceneName.Game);
    }

    public void OnRestartButtonClicked()
    {
        SceneLoader.Load(SceneLoader.SceneName.Game);
    }

    public void OnResumeButtonClicked()
    {
        TogglePause();
    }

    public void OnBackToMenuButtonClicked()
    {
        SceneLoader.Load(SceneLoader.SceneName.Menu);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private enum Type
    {
        Unspecified,
        MainMenu,
        Pause,
        GameOver,
    }

    private enum Int_State
    {
        Start = 0,
        Off = 1,
        On = 2,
    }
}
