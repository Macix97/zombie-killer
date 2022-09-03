using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Header(nameof(Enemy))]
    [SerializeField] private float _attackRange = 1.5f;
    [SerializeField] private float _acceleration = 10.0f;
    [SerializeField] private float _stoppingDistance = 0.25f;

    private Coroutine _decisionCoroutine;

    private static List<Enemy> _alive = new List<Enemy>();
    private static List<Enemy> _corpses = new List<Enemy>();

    public static List<Enemy> Corpses => _corpses;
    public static int EnemyCount => _alive.Count;

    private const float DecisionInterval = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        NavMeshAgent.speed = Speed;
        NavMeshAgent.angularSpeed = AngularSpeed;
        NavMeshAgent.acceleration = _acceleration;
        NavMeshAgent.stoppingDistance = _stoppingDistance;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _alive.Add(this);
        PlayerCharacter.OnPlayerDead += OnPlayerDead;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _alive.Remove(this);
        _corpses.Remove(this);
        PlayerCharacter.OnPlayerDead -= OnPlayerDead;
    }

    protected override void Start()
    {
        base.Start();
        _decisionCoroutine = StartCoroutine(OnDecisionCoroutine());
    }

    protected override void Update()
    {
        base.Update();
        if (!CurrentState.IsDead()) UpdateMovementAnimation();
    }

    private IEnumerator OnDecisionCoroutine()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, DecisionInterval));
        while (true)
        {
            switch (CurrentState)
            {
                case State.Default:
                    UpdateDestination();
                    TryAttackPlayer();
                    break;
            }
            yield return new WaitForSeconds(DecisionInterval);
        }
    }

    protected override void UpdateMovementAnimation()
    {
        float velocityFactor = Mathf.Clamp01((float)NavMeshAgent.velocity.magnitude / (float)NavMeshAgent.speed);
        Animator.SetFloat(AnimatorParameter.Float_Velocity.ToString(), velocityFactor);
    }

    private void UpdateDestination()
    {
        NavMeshAgent.isStopped = false;
        NavMeshAgent.destination = PlayerCharacter.Position;
    }

    private void TryAttackPlayer()
    {
        float squaredMagnitude = Vector3.SqrMagnitude(transform.position - PlayerCharacter.Position);
        if (squaredMagnitude < _attackRange.Pow2())
        {
            CurrentState = State.Attacking;
            NavMeshAgent.isStopped = true;
            PlayerCharacter.TrapPlayer();
            Animator.SetTrigger(AnimatorParameter.Trigger_Attack.ToString());
        }
    }

    public override void Kill(Vector3 sourcePosition)
    {
        if (CurrentState.IsDead()) return;
        base.Kill(sourcePosition);
        _corpses.Add(this);
        _alive.Remove(this);
        StopCoroutine(_decisionCoroutine);
        PlayerCharacter.OnPlayerDead -= OnPlayerDead;
    }

    private void OnPlayerDead()
    {
        NavMeshAgent.isStopped = true;
        StopCoroutine(_decisionCoroutine);
    }

    public void KillPlayer() => PlayerCharacter.KillPlayer(transform.position);
}
