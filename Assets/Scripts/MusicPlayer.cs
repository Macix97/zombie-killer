using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer _instance;

    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioClip _combatMusic;
    [SerializeField] private AudioClip _gameOverMusic;
    [SerializeField] private GameObject _musicSourcePrefab;

    private void Awake()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        if (_instance) Destroy(gameObject);
        else _instance = this;
    }

    private void OnEnable()
    {
        SceneLoader.OnLoadingStarted += PlaySceneMusic;
        PlayerCharacter.OnPlayerDead += PlayGameOverMusic;
    }

    private void OnDisable()
    {
        SceneLoader.OnLoadingStarted -= PlaySceneMusic;
        PlayerCharacter.OnPlayerDead -= PlayGameOverMusic;
    }

    private void Start()
    {
        PlaySceneMusic(SceneLoader.GetSceneName());
    }

    private void PlayGameOverMusic() => PlayMusic(_gameOverMusic, false);

    private void PlaySceneMusic(SceneLoader.SceneName sceneName)
    {
        if (TryGetSceneMusic(sceneName, out AudioClip musicClip)) PlayMusic(musicClip, true);
    }

    private void PlayMusic(AudioClip musicClip, bool loop)
    {
        MusicSource.StopPlayingAll();
        MusicSource musicSource = Instantiate(_musicSourcePrefab, transform).GetComponent<MusicSource>();
        musicSource.StartPlaying(musicClip, loop);
    }

    private bool TryGetSceneMusic(SceneLoader.SceneName sceneName, out AudioClip musicClip)
    {
        musicClip = GetSceneMusic(sceneName);
        return musicClip;
    }

    private AudioClip GetSceneMusic(SceneLoader.SceneName sceneName)
    {
        switch (sceneName)
        {
            case SceneLoader.SceneName.Menu: return _menuMusic;
            case SceneLoader.SceneName.Game: return _combatMusic;
            default: return null;
        }
    }
}
