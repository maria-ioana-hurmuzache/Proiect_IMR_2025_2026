using UnityEngine;

public class FloatingStep : MonoBehaviour
{
    private Rigidbody rb;
    public float targetHeight = 0.6f; // Înălțimea medie a ochilor în metri
    public float floatStrength = 5f;  // Cât de repede revine la nivelul ochilor
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void FixedUpdate()
    {
        // Dacă NU ținem obiectul în mână, aplicăm forța de plutire
        if (grab != null && !grab.isSelected)
        {
            // Calculăm diferența de înălțime
            float heightError = targetHeight - transform.position.y;
            
            // Aplicăm o forță verticală proporțională cu eroarea (ca un resort)
            Vector3 liftingForce = Vector3.up * heightError * floatStrength;
            
            rb.AddForce(liftingForce, ForceMode.Acceleration);
        }
    }
}