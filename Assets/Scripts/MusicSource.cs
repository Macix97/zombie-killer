using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
    [SerializeField] private float _transitionTime = 0.5f;

    private float _musicVelocity;
    private float _targetVolume;
    private float _currentVolume;
    private AudioSource _audioSource;

    private const float DestroyVolumeThreshold = 0.1F;

    private static List<MusicSource> _all = new List<MusicSource>();

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _all.Add(this);
    }

    private void OnDisable()
    {
        _all.Remove(this);
    }

    private void Update()
    {
        _currentVolume = Mathf.SmoothDamp(_currentVolume, _targetVolume, ref _musicVelocity, _transitionTime);
        _audioSource.volume = _currentVolume;
    }

    public void StartPlaying(AudioClip musicClip, bool loop)
    {
        _targetVolume = 1.0f;
        _audioSource.loop = loop;
        _audioSource.volume = 0.0f;
        _audioSource.clip = musicClip;
        _audioSource.Play();
    }

    public static void StopPlayingAll()
    {
        _all.ForEach(s => s.StopPlaying());
    }

    private void StopPlaying()
    {
        _targetVolume = 0.0f;
        StartCoroutine(DestroyOnDelay());
    }

    private IEnumerator DestroyOnDelay()
    {
        yield return new WaitWhile(() => _currentVolume > DestroyVolumeThreshold);
        Destroy(gameObject);
    }
}
