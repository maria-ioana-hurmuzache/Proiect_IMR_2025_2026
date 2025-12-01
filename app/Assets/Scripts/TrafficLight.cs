using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public List<GameObject> redLights;
    public List<GameObject> greenLights;
    public List<GameObject> redPed;
    public List<GameObject> greenPed;

    public float greenDuration = 10f;
    public float redDuration = 10f;

    [HideInInspector]
    public bool isGreen = true;

    private float cycleDuration;

    private void Start()
    {
        cycleDuration = greenDuration + redDuration;
        UpdateLights();
    }

    private void Update()
    {
        float t = Time.time % cycleDuration;

        bool newState = t < greenDuration;
        if (newState != isGreen)
        {
            isGreen = newState;
            UpdateLights();
        }
    }

    private void UpdateLights()
    {
        SetLights(greenLights, isGreen);
        SetLights(redLights, !isGreen);
        SetLights(redPed, isGreen);    // pietoni rosu, masini verde
        SetLights(greenPed, !isGreen); // pietoni verde, masini rosu
    }

    void SetLights(List<GameObject> lights, bool state)
    {
        foreach (var light in lights)
        {
            light.SetActive(state);
        }
    }
}