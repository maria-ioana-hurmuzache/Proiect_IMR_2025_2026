using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameSessionManager : MonoBehaviour
{
    [Header("Referințe XR")]
    public Transform xrCameraTransform; // Trage aici "Main Camera" din XR Origin (Hierarchy)
    
    [Header("Setari Timp")]
    public float timeRemaining = 300f; // 5 min implicit
    public bool isTestMode;

    [Header("Status Task-uri")]
    public bool aVerificatPulsul;
    public bool aSunatLa112;
    public int treceriPeRosu;

    [Header("UI Feedback")]
    public GameObject feedbackCanvas;
    public Text textFeedback;

    private bool jocTerminat;

    void Start()
    {
        if (isTestMode) timeRemaining = 420f; // 7 min pentru test
        feedbackCanvas.SetActive(false);
    }

    void Update()
    {
        if (jocTerminat) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
        }
        else
        {
            FinalizeazaJocul("Timpul a expirat!");
        }
    }

    // Apelează asta din butoanele tale preexistente
    public void TaskPulsCompletat() => aVerificatPulsul = true;
    public void Task112Completat() => aSunatLa112 = true;

    // Apelează asta din logica de trafic
    public void JucatorTrecutPeRosu()
    {
        treceriPeRosu++;
        // Resetare poziție jucător aici dacă e necesar
        Debug.Log("Jucătorul a încercat să treacă pe roșu!");
    }
    
    public void PozitioneazaCanvasInFataJucatorului()
    {
        if (xrCameraTransform == null)
        {
            // Fallback în caz că ai uitat să o tragi în Inspector
            if (Camera.main != null) xrCameraTransform = Camera.main.transform;
            else return;
        }

        Vector3 spawnPos = xrCameraTransform.position + xrCameraTransform.forward * 2.0f;
        spawnPos.y = 1.3f; 

        feedbackCanvas.transform.position = spawnPos;
    
        // Direcționăm UI-ul spre jucător
        Vector3 targetPostion = new Vector3(xrCameraTransform.position.x, feedbackCanvas.transform.position.y, xrCameraTransform.position.z);
        feedbackCanvas.transform.LookAt(targetPostion);
        feedbackCanvas.transform.Rotate(0, 180, 0); 
    }

    public void FinalizeazaJocul(string mesajExtra = "")
    {
        jocTerminat = true;
        PozitioneazaCanvasInFataJucatorului();
        feedbackCanvas.SetActive(true);

        string raport = $"Rezultat Final:\n" +
                        $"- Puls verificat: {(aVerificatPulsul ? "DA" : "NU")}\n" +
                        $"- Apel 112: {(aSunatLa112 ? "DA" : "NU")}\n" +
                        $"- Treceri pe roșu: {treceriPeRosu}\n" +
                        mesajExtra;
        
        textFeedback.text = raport;
        Time.timeScale = 0; // Oprește simularea
    }

    public void InapoiLaMeniu()
    {
        Time.timeScale = 1; // Resetăm timpul înainte de schimbarea scenei
        SceneManager.LoadScene("MainMenu"); // Pune numele corect al scenei tale
    }
}