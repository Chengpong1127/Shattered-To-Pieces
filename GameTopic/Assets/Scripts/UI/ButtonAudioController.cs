using DigitalRuby.SoundManagerNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonAudioController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public AudioClip ClickSound;
    [SerializeField] [Range(0, 1)] public float ClickSoundVolume = 1;

    [SerializeField] public AudioClip HoverSound;
    [SerializeField] [Range(0, 1)] public float HoverSoundVolume = 1;
    [SerializeField] public AudioClip HoverOutSound;
    [SerializeField] [Range(0, 1)] public float HoverOutSoundVolume = 1;
    private AudioSource _audioSource;
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ClickSound != null)
        {
            _audioSource.PlayOneShotSoundManaged(ClickSound, ClickSoundVolume);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HoverSound != null)
        {
            _audioSource.PlayOneShotSoundManaged(HoverSound, HoverSoundVolume);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (HoverOutSound != null)
        {
            _audioSource.PlayOneShotSoundManaged(HoverOutSound, HoverOutSoundVolume);
        }
    }
}