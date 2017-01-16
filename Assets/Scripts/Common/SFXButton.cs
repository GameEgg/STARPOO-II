using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SFXButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    AudioClip btnClickAudio;

    [SerializeField]
    AudioClip btnMouseOverAudio;

    Button btn;

    public void Start()
    {
        btn = gameObject.GetComponent<Button>();
        if (btnClickAudio != null)
            btn.onClick.AddListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(btnMouseOverAudio != null && btn.interactable)
            PlayMouseOverSound();
    }

    public void PlayClickSound()
    {
        SFXManager.instance.Play(btnClickAudio);
    }

    public void PlayMouseOverSound()
    {
        SFXManager.instance.Play(btnMouseOverAudio);
    }
}
