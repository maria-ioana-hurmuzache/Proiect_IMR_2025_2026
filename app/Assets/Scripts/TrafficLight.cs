using UnityEngine;
using System.Collections.Generic;

public class TrafficLight : MonoBehaviour
{
    // Light objects for cars (RED, YELLOW, GREEN)
    public List<GameObject> redLights;
    public List<GameObject> yellowLights;
    public List<GameObject> greenLights;

    // Light objects for pedestrians (RED, GREEN)
    public List<GameObject> redPed;
    public List<GameObject> greenPed;

    // Phase durations (adjustable in the Inspector)
    public float greenDuration = 10f;
    public float yellowToRedDuration = 3f; // Warning yellow (transitioning from Green)
    public float redDuration = 10f;        // Full red (for cars)
    public float redToGreenDuration = 2f;  // Red + Yellow together (Preparation phase)

    // Traffic cycle phases
    public enum LightPhase { Green, YellowToRed, Red, RedAndYellowToGreen };
    [HideInInspector]
    public LightPhase currentPhase;

    private float cycleDuration; // Total duration of one cycle

    private void Start()
    {
        // Total duration: Green + Yellow1 + Red + Red&Yellow
        cycleDuration = greenDuration + yellowToRedDuration + redDuration + redToGreenDuration;
        currentPhase = LightPhase.Green;
        UpdateLights();
    }

    private void Update()
    {
        float t = Time.time % cycleDuration;
        LightPhase newPhase;

        // --- Yellow Phase Logic (with 4 phases) ---
        float timeY1 = greenDuration; // Transition to Warning Yellow
        float timeR = timeY1 + yellowToRedDuration; // Transition to Full Red
        float timeRY2 = timeR + redDuration; // Transition to Red & Yellow

        // 1. Green Phase (Cars)
        if (t < timeY1)
        {
            newPhase = LightPhase.Green;
        }
        // 2. Warning Yellow Phase (Green -> Red)
        else if (t < timeR)
        {
            newPhase = LightPhase.YellowToRed;
        }
        // 3. Red Phase (Cars) - Green for Pedestrians
        else if (t < timeRY2)
        {
            newPhase = LightPhase.Red;
        }
        // 4. Red & Yellow Phase (Preparation -> Green)
        else
        {
            newPhase = LightPhase.RedAndYellowToGreen;
        }

        // Check if the phase has changed
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            UpdateLights();
        }
    }

    private void UpdateLights()
    {
        // Turn off all lights at the beginning of each transition
        SetLights(greenLights, false);
        SetLights(yellowLights, false);
        SetLights(redLights, false);
        SetLights(redPed, false);
        SetLights(greenPed, false);

        switch (currentPhase)
        {
            case LightPhase.Green:
                // Cars: GREEN; Pedestrians: RED
                SetLights(greenLights, true);
                SetLights(redPed, true);
                break;

            case LightPhase.YellowToRed:
                // Cars: YELLOW; Pedestrians: RED (Yellow is short, pedestrians stay on red)
                SetLights(yellowLights, true);
                SetLights(redPed, true);
                break;

            case LightPhase.Red:
                // Cars: RED; Pedestrians: GREEN
                SetLights(redLights, true);
                SetLights(greenPed, true);
                break;

            case LightPhase.RedAndYellowToGreen:
                // Cars: RED & YELLOW; Pedestrians: RED (Pedestrians must have red light before cars get green)
                SetLights(redLights, true);
                SetLights(yellowLights, true);
                SetLights(redPed, true);
                break;
        }
    }

    // Helper function to toggle lists of light objects
    void SetLights(List<GameObject> lights, bool state)
    {
        if (lights != null)
        {
            foreach (var streetLight in lights)
            {
                if (streetLight != null)
                {
                    streetLight.SetActive(state);
                }
            }
        }
    }
    
    public GameSessionManager gameManager; 

    // New method to check if it is safe to cross
    public bool IsSafeToCross() {
        // In this logic, pedestrians have GREEN only when cars have RED
        return currentPhase == LightPhase.Red; 
    }
}