using UnityEngine;

public class UIAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _clickButtonClip;
    [SerializeField] private AudioClip _selectButtonClip;

    public void PlayClickButtonClip()
    {
        SoundsManager.PlayAudioClip(_clickButtonClip);
    }

    public void PlaySelectButtonClip()
    {
        SoundsManager.PlayAudioClip(_selectButtonClip);
    }
}
