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
    
    [SerializeField] private bool useLevelUnlock = false;
    private int unlockedWeaponCount = 1;
    private Image[] slotImages = new Image[6]; 
    private RectTransform highlight;

    void Start()
    {
        if (useLevelUnlock)
        {
            int level = 1;
            if (SaveManager.Instance != null && SaveManager.Instance.playerStats != null)
            {
                level = SaveManager.Instance.playerStats.currentLevel;
            }

            unlockedWeaponCount = 1;
            if (level >= 3) unlockedWeaponCount = 2;
            if (level >= 8) unlockedWeaponCount = 3;
            
            unlockedWeaponCount = Mathf.Min(unlockedWeaponCount, inventory.Count);
        }
        else
        {
            unlockedWeaponCount = inventory.Count;
        }

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
        if (unlockedWeaponCount > 0) SelectWeapon(0);
    }

    void Update()
    {
        for (int i = 0; i < unlockedWeaponCount; i++)
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

            if (i < unlockedWeaponCount)
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