using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Debug.Log("Pressed the main menu");
        SceneManager.LoadScene("MainMenu");
    }
}