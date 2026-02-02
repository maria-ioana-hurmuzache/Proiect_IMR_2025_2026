using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    public Transform xrCameraTransform;
    
    public float timeRemaining = 30f;
    public bool isTestMode;

    public bool hasVerifiedPulse;
    public bool hasMadeEmergencyCall;
    public int illegalCrossingsCounter;
    public CprPressTracker cprPressTracker;

    public GameObject feedbackCanvas;
    public TextMeshProUGUI textFeedback;

    private bool finishedGame;

    void Start()
    {
        if (isTestMode) timeRemaining = 42f;
        feedbackCanvas.SetActive(false);
    }

    void Update()
    {
        if (finishedGame) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            FinishGame();
        }
    }

    public void SetHasVerifiedPulse() => hasVerifiedPulse = true;
    public void SetHasMadeEmergencyCall() => hasMadeEmergencyCall = true;

    public void PlayerCrossedIllegally()
    {
        illegalCrossingsCounter++;
    }
    
    private void PositionCanvasInFrontOfPlayer()
    {
        if (xrCameraTransform == null)
        {
            if (Camera.main != null) xrCameraTransform = Camera.main.transform;
            else return;
        }

        Vector3 spawnPos = xrCameraTransform.position + xrCameraTransform.forward * 2.0f;
        spawnPos.y = 1.3f; 

        feedbackCanvas.transform.position = spawnPos;
    
        Vector3 targetPosition = new Vector3(xrCameraTransform.position.x, feedbackCanvas.transform.position.y, xrCameraTransform.position.z);
        feedbackCanvas.transform.LookAt(targetPosition);
        feedbackCanvas.transform.Rotate(0, 180, 0); 
    }

    private void FinishGame()
    {
        finishedGame = true;
        PositionCanvasInFrontOfPlayer();
        feedbackCanvas.SetActive(true);
        
        float cprScore = (cprPressTracker != null) ? cprPressTracker.GetCprScore() : 0f;
        
        string report = $"Time has expired!\n" +
                        $"Feedback:\n" +
                        $"- Player has crossed the street illegally {illegalCrossingsCounter} times\n" +
                        $"- Player has{(hasVerifiedPulse ? " " : " not ")} verified the victim's pulse\n" +
                        $"- Player has{(hasMadeEmergencyCall ? " " : " not ")} made the emergency call\n" +
                        $"- Player has{(cprScore > 0 ? " " : " not ")} performed CPR\n" +
                        $"- CPR Score : {cprScore}%\n"
                        ;
        
        textFeedback.text = report;
        Time.timeScale = 0;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}