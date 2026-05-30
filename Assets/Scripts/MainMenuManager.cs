using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("UpgradeMenuScene"); 
    }

    public void QuitGame()
    {
        Debug.Log("Đã thoát game thành công!"); 
        Application.Quit();
    }
}