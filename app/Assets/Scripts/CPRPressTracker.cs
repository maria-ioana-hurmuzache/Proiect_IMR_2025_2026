using UnityEngine;

public class CprPressTracker : MonoBehaviour
{
    public Transform hand; // assign the hand XR controller
    public float maxPressDepth = 0.1f; // how deep the press can go
    public float minPressDepth; // no press
    public UnityEngine.UI.Slider pressureBar; // UI slider to show pressure

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (hand == null || pressureBar == null) return;
        
        // Calculate how deep the hand is pressing
        float pressDepth = Mathf.Clamp(originalPosition.y - hand.position.y, minPressDepth, maxPressDepth);
        
        // Move chest down to simulate press
        transform.position = Vector3.Lerp(transform.position, originalPosition - new Vector3(0, pressDepth, 0), Time.deltaTime * 10f);
        
        // Update UI bar (0 = not pressed, 1 = max press)
        pressureBar.value = pressDepth / maxPressDepth;
        
        if (pressureBar.value < 0.3f)
            pressureBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.yellow; // too light
        else if (pressureBar.value > 0.7f)
            pressureBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.red; // too hard
        else
            pressureBar.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.green; // correct
    }
}