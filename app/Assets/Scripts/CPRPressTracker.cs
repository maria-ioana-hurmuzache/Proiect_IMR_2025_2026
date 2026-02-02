using System.Collections.Generic;
using UnityEngine;

public class CprPressTracker : MonoBehaviour
{
    public Transform hand;
    public float maxPressDepth = 0.1f;
    public float minPressDepth = 0f;
    public UnityEngine.UI.Slider pressureBar;

    private Vector3 originalPosition;
    private List<float> allPresses = new List<float>();
    private int correctTempoPresses;
    
    private bool isPressing;
    private float currentMaxInThisPress;
    private float lastPressTime;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (hand == null || pressureBar == null) return;
        
        float pressDepth = Mathf.Clamp(originalPosition.y - hand.position.y, minPressDepth, maxPressDepth);
        transform.position = Vector3.Lerp(transform.position, originalPosition - new Vector3(0, pressDepth, 0), Time.deltaTime * 10f);
        
        float pressureValue = pressDepth / maxPressDepth;
        pressureBar.value = pressureValue;

        if (pressureValue < 0.3f)
            pressureBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.yellow;
        else if (pressureValue > 0.7f)
            pressureBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        else
            pressureBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.green;

        if (pressureValue > 0.1f) 
        {
            if (!isPressing)
            {
                // Calculate frequency (time since last press)
                float timeSinceLast = Time.time - lastPressTime;
                if (timeSinceLast >= 0.5f && timeSinceLast <= 0.6f) 
                {
                    correctTempoPresses++;
                }
                lastPressTime = Time.time;
            }
            isPressing = true;
            if (pressureValue > currentMaxInThisPress) currentMaxInThisPress = pressureValue;
        }
        else if (pressureValue < 0.05f && isPressing) 
        {
            isPressing = false;
            allPresses.Add(currentMaxInThisPress);
            currentMaxInThisPress = 0f;
        }
    }

    public float GetCprScore()
    {
        if (allPresses.Count == 0) return 0f;

        int correctDepthPresses = 0;
        foreach (float press in allPresses)
        {
            if (press >= 0.3f && press <= 0.7f) correctDepthPresses++;
        }

        // Calculate Depth Accuracy (50% weight)
        float depthScore = ((float)correctDepthPresses / allPresses.Count) * 50f;
        
        // Calculate Tempo Accuracy (50% weight)
        float tempoScore = ((float)correctTempoPresses / allPresses.Count) * 50f;

        return depthScore + tempoScore;
    }

    public int GetTotalPresses() => allPresses.Count;
}