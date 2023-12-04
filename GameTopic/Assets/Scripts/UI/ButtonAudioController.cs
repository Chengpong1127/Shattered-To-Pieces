using DigitalRuby.SoundManagerNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class ButtonAudioController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public AudioClip ClickSound;
    [SerializeField]
    public AudioClip HoverSound;
    [SerializeField]
    public AudioClip HoverOutSound;
    private AudioSource _audioSource;
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (ClickSound != null)
        {
            _audioSource.PlayOneShotSoundManaged(ClickSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (HoverSound != null)
        {
            _audioSource.PlayOneShotSoundManaged(HoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (HoverOutSound != null)
        {
            _audioSource.PlayOneShotSoundManaged(HoverOutSound);
        }
    }
}