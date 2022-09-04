using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(CapsuleCollider))]
public class Entity : MonoBehaviour
{
    [Header(nameof(Entity))]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private float _angularSpeed = 180.0f;
    [SerializeField] private AudioClip _deadClip;
    [SerializeField] private GameObject _hitEffectPrefab;

    private Animator _animator;
    private CapsuleCollider _collider;
    private NavMeshAgent _navMeshAgent;

    protected State CurrentState;

    protected float Speed => _speed;
    protected float AngularSpeed => _angularSpeed;
    protected Animator Animator => _animator;
    protected AudioClip DeadClip => _deadClip;
    protected NavMeshAgent NavMeshAgent => _navMeshAgent;

    public Vector3 HitEffectPosition => transform.position + Vector3.up * _collider.height * 0.5f;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<CapsuleCollider>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start() { }

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

    protected virtual void Update() { }

    protected virtual void UpdateMovementAnimation() { }

    public virtual void Kill(Vector3 sourcePosition)
    {
        CurrentState = State.Dead;
        _collider.enabled = false;
        _navMeshAgent.enabled = false;
        SpawnHitEffect(sourcePosition);
        Animator.SetTrigger(AnimatorParameter.Trigger_Dead.ToString());
        SoundsManager.PlayAudioClip(DeadClip, true, transform.position);
    }

    private void SpawnHitEffect(Vector3 sourcePosition)
    {
        if (!_hitEffectPrefab) return;
        Vector3 hitEffectDirection = Vector3.Normalize(sourcePosition.Flat() - transform.position.Flat());
        Quaternion hitEffectRotation = Quaternion.LookRotation(hitEffectDirection);
        Destroy(Instantiate(_hitEffectPrefab, HitEffectPosition, hitEffectRotation), GameSettings.EffectLifeTime);
    }

    protected enum AnimatorParameter
    {
        Float_X,
        Float_Y,
        Trigger_Dead,
        Trigger_Attack,
        Trigger_Trapped,
        Float_Velocity,
    }

    public enum State
    {
        Default,
        Trapped,
        Attacking,
        Dead,
    }
}
