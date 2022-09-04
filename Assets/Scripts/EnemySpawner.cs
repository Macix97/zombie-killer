using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;

    private Coroutine _spawnerCoroutine;

    private List<Enemy> _spawnedEnemies = new List<Enemy>();

    private void OnEnable()
    {
        Enemy.OnEnemyDead += ClearCorpses;
        PlayerCharacter.OnPlayerDead += StopSpawner;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDead -= ClearCorpses;
        PlayerCharacter.OnPlayerDead -= StopSpawner;
    }

    private void Start()
    {
        _spawnerCoroutine = StartCoroutine(OnSpawnerCoroutine());
    }

    private IEnumerator OnSpawnerCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameSettings.SpawnInterval);
            Vector3 spawnPosition = GetRandomSpawnPosition();
            Quaternion spawnRotation = PlayerCharacter.RotationToPlayer(spawnPosition);
            Instantiate(_enemyPrefab, spawnPosition, spawnRotation);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        SpawnPoint[] validSpawnPoints = SpawnPoint.All.Where(p => !PlayerCamera.IsCameraSeeTransform(p.transform)).ToArray();
        return validSpawnPoints.RandomElement().transform.position;
    }

    private void ClearCorpses()
    {
        if (Enemy.Corpses.Count <= GameSettings.MaxCorpsesOnScene) return;
        if (Enemy.Corpses.TryFind(e => !PlayerCamera.IsCameraSeeTransform(e.transform), out Enemy enemy)) Destroy(enemy.gameObject);
        else Destroy(Enemy.Corpses.RandomElement().gameObject);
    }

    private void StopSpawner()
    {
        StopCoroutine(_spawnerCoroutine);
    }
}
