using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button trainingButton; // Training/Tutorial button
    public Button testButton;     // Test button


    public List<Button> returnButtons;

    void Start()
    {
        Time.timeScale = 1f;
        EnableMainMenu();

        if (trainingButton) trainingButton.onClick.AddListener(StartTraining);
        if (testButton) testButton.onClick.AddListener(StartTest);
    }

    private void StartTraining()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    private void StartTest()
    {
        HideAll();
        SceneTransitionManager.singleton.GoToSceneAsync(2);
    }

    private void HideAll()
    {
        mainMenu.SetActive(false);
    }

    private void EnableMainMenu()
    {
        mainMenu.SetActive(true);
    }
    
}
