using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI textProgress;

    public void Setup(Sprite itemSprite, int current, int target)
    {
        icon.sprite = itemSprite;
        textProgress.text = current + " / " + target;

        textProgress.color = (current >= target) ? Color.green : Color.white;
    }
}