using UnityEngine;

[CreateAssetMenu(fileName = nameof(GameSettings))]
public class GameSettings : ScriptableObject
{
    private static GameSettings _instance;

    [SerializeField] private int _maxCorpsesOnScene = 40;
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private float _gameOverDelay = 5.0f;
    [SerializeField] private float _sceneFadingTime = 0.5f;
    [SerializeField] private float _effectLifeTime = 5.0f;
    [SerializeField] private Color _highlightColor = Color.white;
    [SerializeField] private Color _nonHighlightColor = Color.black;

    public static int MaxCorpsesOnScene => Instance._maxCorpsesOnScene;
    public static float SpawnInterval => Instance._spawnInterval;
    public static float GameOverDelay => Instance._gameOverDelay;
    public static float SceneFadingTime => Instance._sceneFadingTime;
    public static float EffectLifeTime => Instance._effectLifeTime;
    public static Color HighlightColor => Instance._highlightColor;
    public static Color NonHighlightColor => Instance._nonHighlightColor;

    private static GameSettings Instance
    {
        get
        {
            if (!_instance) _instance = Resources.Load<GameSettings>(nameof(GameSettings));
            return _instance;
        }
    }

    public const string EnemyTag = "Enemy";
    public const string EmissionColor = "_EmissionColor";
}
