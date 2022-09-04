using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerCharacter : Entity
{
    private static PlayerCharacter _instance;

    [Header(nameof(PlayerCharacter))]
    [SerializeField] private float _fireRate = 2.0f;
    [SerializeField] private float _accelerationTime = 1.0f;
    [SerializeField] private Transform _projectilePoint;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _fireEffectPrefab;
    [SerializeField] private AudioClip _fireClip;

    private bool _isFireCooldown;
    private Vector2 _currentMoveInput;
    private Vector2 _moveInputVelocity;
    private Vector3 _currentMoveDirection;
    private Vector3 _moveDirectionVelocity;

    public static event Action OnPlayerDead;

    public static Transform Transform => _instance ? _instance.transform : null;
    public static Vector3 Position => _instance ? _instance.transform.position : Vector3.zero;

    protected override void Awake()
    {
        base.Awake();
        _instance = this;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InputManager.OnFire += PerformFire;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        InputManager.OnFire -= PerformFire;
    }

    protected override void Update()
    {
        base.Update();
        if (!CurrentState.IsDefault()) return;
        UpdateMovePosition();
        UpdateLookRotation();
        UpdateMovementAnimation();
    }

    private void PerformFire()
    {
        if (_isFireCooldown || !CurrentState.IsDefault()) return;
        StartCoroutine(WaitForFireCooldown());
        Instantiate(_projectilePrefab, _projectilePoint.position, transform.rotation);
        Destroy(Instantiate(_fireEffectPrefab, _projectilePoint.position, transform.rotation, _projectilePoint), GameSettings.EffectLifeTime);
        SoundsManager.PlayAudioClip(_fireClip, randomPitch: true);
    }

    private void UpdateMovePosition()
    {
        Vector3 targetMoveDirection = transform.right * InputManager.MoveInput.x + transform.forward * InputManager.MoveInput.y;
        _currentMoveDirection = Vector3.SmoothDamp(_currentMoveDirection, targetMoveDirection, ref _moveDirectionVelocity, _accelerationTime);
        NavMeshAgent.Move(_currentMoveDirection * Time.deltaTime * Speed);
    }

    protected override void UpdateMovementAnimation()
    {
        _currentMoveInput = Vector2.SmoothDamp(_currentMoveInput, InputManager.MoveInput, ref _moveInputVelocity, _accelerationTime);
        Animator.SetFloat(AnimatorParameter.Float_X.ToString(), _currentMoveInput.x);
        Animator.SetFloat(AnimatorParameter.Float_Y.ToString(), _currentMoveInput.y);
    }

    private void UpdateLookRotation()
    {
        Vector3 cursorWorldPosition = PlayerCamera.GetCursorWorldPosition();
        Vector3 lookDirection = Vector3.Normalize(cursorWorldPosition - transform.position.Flat());
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * AngularSpeed);
    }

    private IEnumerator WaitForFireCooldown()
    {
        _isFireCooldown = true;
        yield return new WaitForSeconds(_fireRate);
        _isFireCooldown = false;
    }

    public static Quaternion RotationToPlayer(Vector3 position)
    {
        Vector3 lookDirection = Vector3.Normalize(position.Flat() - Position.Flat());
        return Quaternion.LookRotation(lookDirection);
    }

    public static void TrapPlayer()
    {
        if (!_instance || !_instance.CurrentState.IsDefault()) return;
        _instance.CurrentState = State.Trapped;
        _instance.Animator.SetTrigger(AnimatorParameter.Trigger_Trapped.ToString());
    }

    public static void KillPlayer(Vector3 sourcePosition)
    {
        if (_instance) _instance.Kill(sourcePosition);
    }

    public override void Kill(Vector3 sourcePosition)
    {
        if (CurrentState.IsDead()) return;
        base.Kill(sourcePosition);
        OnPlayerDead?.Invoke();
    }
}
