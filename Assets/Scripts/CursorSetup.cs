using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CursorSetup))]
public class CursorSetup : ScriptableObject
{
    private static CursorSetup _instance;

    [SerializeField] private Cursor _gameCursor;
    [SerializeField] private Cursor _menuCursor;

    public static Cursor GameCursor => Instance._gameCursor;
    public static Cursor MenuCursor => Instance._menuCursor;
    private static CursorSetup Instance
    {
        get
        {
            if (!_instance) _instance = Resources.Load<CursorSetup>(nameof(CursorSetup));
            return _instance;
        }
    }

    [Serializable]
    public struct Cursor
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private Vector2 _hotspot;

        public Texture2D Texture => _texture;
        public Vector2 Hotspot => _hotspot;
    }
}