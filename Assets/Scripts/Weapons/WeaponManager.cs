using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class WeaponData
{
    public GameObject physicalGun;
    public Sprite uiIcon;
}

public class WeaponManager : MonoBehaviour
{
    public List<WeaponData> inventory = new List<WeaponData>();
    
    private Image[] slotImages = new Image[6]; 
    private RectTransform highlight;

    void Start()
    {
        GameObject hlObj = GameObject.Find("Highlight");
        if (hlObj != null) 
        {
            highlight = hlObj.GetComponent<RectTransform>();
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject slotObj = GameObject.Find("InventoryItem" + i);
            if (slotObj != null)
            {
                slotImages[i] = slotObj.GetComponent<Image>();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy UI tên là 'Slot" + i + "' ngoài Scene!");
            }
        }

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
            if (inventory[i].physicalGun != null)
            {
                inventory[i].physicalGun.SetActive(i == index);
            }
        }

        if (highlight != null && index < slotImages.Length && slotImages[index] != null)
        {
            highlight.position = slotImages[index].rectTransform.position;
        }
    }

    void RefreshUI()
    {
        for (int i = 0; i < slotImages.Length; i++)
        {
            if (slotImages[i] == null) continue; 

            if (i < inventory.Count)
            {
                slotImages[i].sprite = inventory[i].uiIcon;
                slotImages[i].color = Color.white;
            }
            else
            {
                slotImages[i].color = new Color(0, 0, 0, 0); // Làm trong suốt ô trống
            }
        }
    }
}