using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader _instance;

    private float _fadingTime;
    private Image _fadeImage;

    public static event Action<SceneName> OnLoadingStarted;

    private const int SortingOrder = 100;

    public static void Load(SceneName sceneName) => Load(sceneName, GameSettings.SceneFadingTime);

    public static void Load(SceneName sceneName, float fadingTime)
    {
        if (_instance) return;
        OnLoadingStarted?.Invoke(sceneName);
        _instance = new GameObject(nameof(SceneLoader)).AddComponent<SceneLoader>();
        DontDestroyOnLoad(_instance.gameObject);
        Canvas canvas = _instance.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = SortingOrder;
        _instance._fadeImage = _instance.gameObject.AddComponent<Image>();
        _instance._fadeImage.color = Color.black;
        _instance._fadingTime = fadingTime;
        _instance.StartCoroutine(_instance.OnLoadingProcess(sceneName.ToString()));
    }

    private IEnumerator OnLoadingProcess(string sceneName)
    {
        yield return OnFadeOut();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            if (!asyncOperation.allowSceneActivation && asyncOperation.progress >= 0.9f) asyncOperation.allowSceneActivation = true;
            yield return new WaitForEndOfFrame();
        }
        yield return OnFadeIn();
        Destroy(gameObject);
    }

    private IEnumerator OnFadeOut()
    {
        float alpha = 0.0f;
        _fadeImage.enabled = true;
        while (!Mathf.Approximately(alpha, 1.0f))
        {
            alpha += Time.unscaledDeltaTime / _fadingTime;
            SetFadeImageAlpha(ref alpha);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator OnFadeIn()
    {
        float alpha = 1.0f;
        while (!Mathf.Approximately(alpha, 0.0f))
        {
            alpha -= Time.unscaledDeltaTime / _fadingTime;
            SetFadeImageAlpha(ref alpha);
            yield return new WaitForEndOfFrame();
        }
        _fadeImage.enabled = false;
    }

    private void SetFadeImageAlpha(ref float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, alpha);
    }

    public static SceneName GetSceneName()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        return Enum.TryParse<SceneName>(activeSceneName, true, out SceneName sceneName) ? sceneName : SceneName.Unspecified;
    }

    public enum SceneName
    {
        Unspecified,
        Menu,
        Game,
    }
}
