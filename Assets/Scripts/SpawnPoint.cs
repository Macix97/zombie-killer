using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private float _gizmosSize = 0.5f;
    [SerializeField] private Color _gizmosColor = Color.red;

    private static List<SpawnPoint> _all = new List<SpawnPoint>();

    public static List<SpawnPoint> All => _all;

    private void OnEnable()
    {
        _all.Add(this);
    }

    private void OnDisable()
    {
        _all.Remove(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmosColor;
        Gizmos.DrawWireSphere(transform.position, _gizmosSize);
        Gizmos.color = new Color(_gizmosColor.r, _gizmosColor.g, _gizmosColor.b, 0.5f);
        Gizmos.DrawSphere(transform.position, _gizmosSize);
    }
}
