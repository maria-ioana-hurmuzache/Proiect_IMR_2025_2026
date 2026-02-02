using UnityEngine;

public class FloatingStep : MonoBehaviour
{
    private Rigidbody rb;
    public float targetHeight = 0.6f; // Average sight height
    public float floatStrength = 5f;  // How fast it reaches eye level again
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void FixedUpdate()
    {
        // If object is not grabbed then it floats
        if (grab != null && !grab.isSelected)
        {
            // Compute the height difference
            float heightError = targetHeight - transform.position.y;
            
            // Apply a vertical force equal to the height difference (similar to a resort)
            Vector3 liftingForce = Vector3.up * heightError * floatStrength;
            
            rb.AddForce(liftingForce, ForceMode.Acceleration);
        }
    }
}