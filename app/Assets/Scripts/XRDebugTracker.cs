using UnityEngine;

public class XRDebugTracker : MonoBehaviour
{
    private Vector3 startGlobalPos;
    private Vector3 startLocalPos;

    void Awake()
    {
        startGlobalPos = transform.position;
        startLocalPos = transform.localPosition;
        Debug.Log($"<color=cyan>[Diagnostic] {name} Awake.</color> Global: {startGlobalPos}, Local: {startLocalPos}");
    }

    void Start()
    {
        Debug.Log($"<color=cyan>[Diagnostic] {name} Start.</color> Global: {transform.position}");
        
        // Check if the object moved between Awake and Start
        if (transform.position != startGlobalPos)
        {
            Debug.LogWarning($"<color=red>[ALERT] {name} moved between Awake and Start!</color>");
        }
        
        // Check position after XR Interaction Toolkit has fully initialized
        Invoke(nameof(CheckPostInitialization), 2.0f);
    }

    void CheckPostInitialization()
    {
        Debug.Log($"<color=yellow>[Diagnostic] {name} after 2 seconds.</color> Global: {transform.position}");
        
        if (Vector3.Distance(transform.position, startGlobalPos) > 0.01f)
        {
            Debug.LogError($"<color=red>[Diagnostic] {name} was forced to move by the system! Current Position: {transform.position}, Expected: {startGlobalPos}</color>");
        }
        else
        {
            Debug.Log("<color=green>[Diagnostic] Position is stable.</color>");
        }
    }

    void Update()
    {
        // If the object disappears suddenly, check if it flew to extreme coordinates
        if (transform.localPosition.magnitude > 1000) 
            Debug.LogError($"{name} has flown to extreme coordinates!");
    }
}