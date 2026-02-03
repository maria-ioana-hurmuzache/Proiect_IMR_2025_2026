using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Pressed the main menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
