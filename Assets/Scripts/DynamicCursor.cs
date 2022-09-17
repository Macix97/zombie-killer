using UnityEngine;

public class DynamicCursor : MonoBehaviour
{
    private static DynamicCursor _instance;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        if (_instance) return;
        _instance = new GameObject(nameof(DynamicCursor)).AddComponent<DynamicCursor>();
        bool isGame = SceneLoader.CurrentSceneName == SceneLoader.SceneName.Game;
        _instance.SetCursor(isGame ? CursorSetup.GameCursor : CursorSetup.MenuCursor);
        DontDestroyOnLoad(_instance.gameObject);
    }

    private void OnEnable()
    {
        Menu.OnPauseToggle += SetCursor;
        SceneLoader.OnLoadingStarted += DisableCursor;
        SceneLoader.OnLoadingFinished += EnableCursor;
    }

    private void OnDisable()
    {
        Menu.OnPauseToggle -= SetCursor;
        SceneLoader.OnLoadingStarted -= DisableCursor;
        SceneLoader.OnLoadingFinished -= EnableCursor;
    }

    private void SetCursor(bool isPause)
    {
        SetCursor(isPause ? CursorSetup.MenuCursor : CursorSetup.GameCursor);
    }

    private void SetCursor(CursorSetup.Cursor cursor)
    {
        Cursor.SetCursor(cursor.Texture, cursor.Hotspot, CursorMode.Auto);
    }

    private void EnableCursor(SceneLoader.SceneName _) => SetCursorActive(true);

    private void DisableCursor(SceneLoader.SceneName _) => SetCursorActive(false);

    private void SetCursorActive(bool active)
    {
        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}
