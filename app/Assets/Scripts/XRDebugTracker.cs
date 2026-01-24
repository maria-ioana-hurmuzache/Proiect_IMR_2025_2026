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
        // Verificăm dacă s-a mutat deja față de Awake
        if (transform.position != startGlobalPos)
        {
            Debug.LogWarning($"<color=red>[ALERTA] {name} s-a mutat între Awake și Start!</color>");
        }
        
        // Verificăm poziția după ce XRIT s-a inițializat complet
        Invoke(nameof(CheckPostInitialization), 2.0f);
    }

    void CheckPostInitialization()
    {
        Debug.Log($"<color=yellow>[Diagnostic] {name} după 2 secunde.</color> Global: {transform.position}");
        if (Vector3.Distance(transform.position, startGlobalPos) > 0.01f)
        {
            Debug.LogError($"<color=red>[Diagnostic] {name} a fost mutat forțat de sistem! Poziție actuală: {transform.position}, Trebuia să fie: {startGlobalPos}</color>");
        }
        else
        {
            Debug.Log("<color=green>[Diagnostic] Poziția este stabilă.</color>");
        }
    }

    void Update()
    {
        // Dacă obiectul dispare brusc, verificăm dacă are Scale 0 sau e foarte departe
        if (transform.localPosition.magnitude > 1000) 
            Debug.LogError($"{name} a zburat la coordonate enorme!");
    }
}