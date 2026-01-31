using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject about;

    [Header("Main Menu Buttons")]
    public Button trainingButton; // Butonul pentru Training/Tutorial
    public Button testButton;     // Butonul pentru Test


    public List<Button> returnButtons;

    void Start()
    {
        EnableMainMenu();

        // Verificăm dacă butoanele sunt asigurate în Inspector și le adăugăm funcționalitatea
        if (trainingButton) trainingButton.onClick.AddListener(StartTraining);
        if (testButton) testButton.onClick.AddListener(StartTest);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    public void StartTraining()
    {
        HideAll();
        // Apelează tranziția către scena de Training (Index 1)
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void StartTest()
    {
        HideAll();
        // Apelează tranziția către scena de Test (Index 2)
        SceneTransitionManager.singleton.GoToSceneAsync(2);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
    }
    
}