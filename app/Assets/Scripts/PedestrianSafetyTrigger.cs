using UnityEngine;

public class PedestrianSafetyTrigger : MonoBehaviour 
{
    public TrafficLight trafficLight;
    public GameSessionManager gameManager;
    public Transform resetPoint;

    private void OnTriggerEnter(Collider other) 
    {
        Transform playerTransform = other.transform.root;

        if (playerTransform.CompareTag("Player") && !trafficLight.IsSafeToCross()) 
        {
            if (gameManager != null) gameManager.JucatorTrecutPeRosu();

            CharacterController cc = playerTransform.GetComponentInChildren<CharacterController>();

            if (cc != null) 
            {
                cc.enabled = false;
                
                playerTransform.position = resetPoint.position;
                playerTransform.rotation = resetPoint.rotation;
                
                Physics.SyncTransforms();
                
                cc.enabled = true;
            }
            else 
            {
                playerTransform.SetPositionAndRotation(resetPoint.position, resetPoint.rotation);
            }
            
            Debug.Log("Pedestrian Safety Trigger Entered");
        }
    }
}