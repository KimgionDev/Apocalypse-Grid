using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip errorSound;

    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (myButton != null && !myButton.interactable) return;

        if (hoverSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hoverSound);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (myButton != null && !myButton.interactable)
        {
            if (errorSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(errorSound);
            }
            return;
        }

        if (clickSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clickSound);
        }
    }
}