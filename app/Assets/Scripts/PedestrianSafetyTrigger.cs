using UnityEngine;

public class PedestrianSafetyTrigger : MonoBehaviour {
    public TrafficLight trafficLight;
    public GameSessionManager gameManager;
    public Transform resetPoint; // Un punct gol unde să fie teleportat jucătorul

    private void OnTriggerEnter(Collider other) 
    {
        // Căutăm XR Origin în părinții obiectului care a atins trigger-ul
        // pentru a teleporta tot sistemul XR, nu doar o mână.
        GameObject playerRoot = other.transform.root.gameObject; 

        if (other.CompareTag("Player") || playerRoot.CompareTag("Player")) 
        { 
            if (!trafficLight.IsSafeToCross()) 
            {
                gameManager.JucatorTrecutPeRosu();
            
                // Teleportăm întreaga structură XR Origin
                playerRoot.transform.position = resetPoint.position;
            
                Debug.Log("Jucător teleportat pentru că a trecut pe roșu!");
            }
        }
    }
}