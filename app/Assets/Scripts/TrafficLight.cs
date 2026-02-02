using UnityEngine;
using System.Collections.Generic;

public class TrafficLight : MonoBehaviour
{
    // Obiectele luminoase pentru masini (RED, YELLOW, GREEN)
    public List<GameObject> redLights;
    public List<GameObject> yellowLights;
    public List<GameObject> greenLights;

    // Obiectele luminoase pentru pietoni (RED, GREEN)
    public List<GameObject> redPed;
    public List<GameObject> greenPed;

    // Duratele fazelor (reglabil in Inspector)
    public float greenDuration = 10f;
    public float yellowToRedDuration = 3f; // Galben de avertizare (dinspre Verde)
    public float redDuration = 10f;        // Rosu complet (pentru masini)
    public float redToGreenDuration = 2f;  // <-- NOU: Rosu + Galben impreuna

    // Fazele ciclului de trafic
    public enum LightPhase { Green, YellowToRed, Red, RedAndYellowToGreen };
    [HideInInspector]
    public LightPhase currentPhase;

    private float cycleDuration; // Durata totala a unui ciclu

    private void Start()
    {
        // Durata totala: Verde + Galben1 + Rosu + Rosu&Galben
        cycleDuration = greenDuration + yellowToRedDuration + redDuration + redToGreenDuration;
        currentPhase = LightPhase.Green;
        UpdateLights();
    }

    private void Update()
    {
        float t = Time.time % cycleDuration;
        LightPhase newPhase;

        // --- Logica Fazei Galbene (cu 4 faze) ---
        float time_Y1 = greenDuration; // Trecere la Galben de Avertizare
        float time_R = time_Y1 + yellowToRedDuration; // Trecere la Rosu complet
        float time_RY2 = time_R + redDuration; // Trecere la Rosu & Galben

        // 1. Faza Verde (Masini)
        if (t < time_Y1)
        {
            newPhase = LightPhase.Green;
        }
        // 2. Faza Galbena de Avertizare (Verde -> Rosu)
        else if (t < time_R)
        {
            newPhase = LightPhase.YellowToRed;
        }
        // 3. Faza Rosie (Masini) - Verde pentru Pietoni
        else if (t < time_RY2)
        {
            newPhase = LightPhase.Red;
        }
        // 4. Faza Rosie & Galben (Pregatire -> Verde)
        else
        {
            newPhase = LightPhase.RedAndYellowToGreen;
        }

        // Verificam daca faza s-a schimbat
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            UpdateLights();
        }
    }

    private void UpdateLights()
    {
        // Stingem toate luminile la inceputul fiecarei tranzitii
        SetLights(greenLights, false);
        SetLights(yellowLights, false);
        SetLights(redLights, false);
        SetLights(redPed, false);
        SetLights(greenPed, false);

        switch (currentPhase)
        {
            case LightPhase.Green:
                // Masini: VERDE; Pietoni: ROSU
                SetLights(greenLights, true);
                SetLights(redPed, true);
                break;

            case LightPhase.YellowToRed:
                // Masini: GALBEN; Pietoni: ROSU (Galbenul e scurt, pietonii raman pe rosu)
                SetLights(yellowLights, true);
                SetLights(redPed, true);
                break;

            case LightPhase.Red:
                // Masini: ROSU; Pietoni: VERDE
                SetLights(redLights, true);
                SetLights(greenPed, true);
                break;

            case LightPhase.RedAndYellowToGreen:
                // Masini: ROSU & GALBEN; Pietoni: ROSU (Pietonii trebuie sa aiba semnalul rosu inainte de verdele masinilor)
                SetLights(redLights, true);
                SetLights(yellowLights, true);
                SetLights(redPed, true);
                break;
        }
    }

    // Functie ajutatoare
    void SetLights(List<GameObject> lights, bool state)
    {
        if (lights != null)
        {
            foreach (var light in lights)
            {
                if (light != null)
                {
                    light.SetActive(state);
                }
            }
        }
    }
    
    // Adaugă la începutul clasei
    public GameSessionManager gameManager; 

// Metodă nouă pentru a verifica dacă e sigur de traversat
    public bool IsSafeToCross() {
        // În logica ta, pietonii au VERDE doar când masinile au ROSU
        return currentPhase == LightPhase.Red; 
    }
}