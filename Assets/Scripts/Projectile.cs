using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5.0f;
    [SerializeField] private float _velocity = 100.0f;
    [SerializeField] private GameObject _hitEffectPrefab;

    private static List<Projectile> _all = new List<Projectile>();

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.velocity = transform.forward * _velocity;
    }

    private void OnEnable()
    {
        _all.Add(this);
    }

    private void OnDisable()
    {
        _all.Remove(this);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_lifeTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        Vector3 hitPoint = collision.contacts[0].point;
        Quaternion lookRotation = Quaternion.LookRotation(-transform.forward);
        Destroy(Instantiate(_hitEffectPrefab, hitPoint, lookRotation), GameSettings.EffectLifeTime);
        if (collision.collider.TryGetComponent<Enemy>(out Enemy enemy)) enemy.Kill(hitPoint);
    }
}
