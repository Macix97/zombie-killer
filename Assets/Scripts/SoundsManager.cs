using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundsManager : MonoBehaviour
{
    private static SoundsManager _instance;

    private AudioSource _templateSource;

    private List<AudioSource> _audioSources = new List<AudioSource>();

    private void Awake()
    {
        _instance = this;
        _templateSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Menu.OnPauseToggle += OnPauseToggle;
    }

    private void OnDisable()
    {
        Menu.OnPauseToggle -= OnPauseToggle;
    }

    public static void PlayAudioClip(AudioClip audioClip, bool randomPitch = false, Vector3 position = default)
    {
        if (!_instance) return;
        AudioSource audioSource = new GameObject(nameof(AudioSource)).AddComponent<AudioSource>();
        audioSource.transform.position = position;
        _instance._audioSources.Add(audioSource);
        _instance.SetSourceParameters(audioSource, audioClip, randomPitch, position);
        audioSource.Play();
        _instance.StartCoroutine(_instance.DestroyAudioSource(audioSource));
    }

    private void SetSourceParameters(AudioSource audioSource, AudioClip audioClip, bool randomPitch, Vector3 position)
    {
        audioSource.clip = audioClip;
        audioSource.pitch = randomPitch ? Random.Range(0.9f, 1.1f) : _templateSource.pitch;
        audioSource.volume = _templateSource.volume;
        audioSource.spatialBlend = position != default ? _templateSource.spatialBlend : 0.0f;
        audioSource.dopplerLevel = _templateSource.dopplerLevel;
        audioSource.spread = _templateSource.spread;
        audioSource.rolloffMode = _templateSource.rolloffMode;
        audioSource.minDistance = _templateSource.minDistance;
        audioSource.maxDistance = _templateSource.maxDistance;
    }

    private IEnumerator DestroyAudioSource(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length * audioSource.pitch);
        Destroy(audioSource.gameObject);
        _audioSources.Remove(audioSource);
    }

    private void OnPauseToggle(bool isPause)
    {
        if (isPause) _audioSources.ForEach(s => s.Pause());
        else _audioSources.ForEach(s => s.UnPause());
    }
}
