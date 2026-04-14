using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class WeaponData
{
    public GameObject physicalGun; // Súng trên tay nhân vật
    public Sprite uiIcon;          // Ảnh hiển thị dưới UI
}

public class WeaponManager : MonoBehaviour
{
    public List<WeaponData> inventory = new List<WeaponData>();
    public Image[] slotImages;       // 6 ô chứa hình
    public RectTransform highlight;  // Khung vàng

    void Start()
    {
        RefreshUI();
        if (inventory.Count > 0) SelectWeapon(0);
    }

    void Update()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) SelectWeapon(i);
        }
    }

    void SelectWeapon(int index)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            inventory[i].physicalGun.SetActive(i == index);
        }

        if (highlight != null && index < slotImages.Length)
        {
            highlight.position = slotImages[index].rectTransform.position;
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (i < inventory.Count)
            {
                slotImages[i].sprite = inventory[i].uiIcon;
                slotImages[i].color = Color.white;
            }
            else
            {
                slotImages[i].color = new Color(0, 0, 0, 0);
            }
        }
    }
}