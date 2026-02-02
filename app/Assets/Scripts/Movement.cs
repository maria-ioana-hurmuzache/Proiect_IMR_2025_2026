using UnityEngine;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public List<Transform> waypoints;
    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public float minDistance = 0.5f;

    private int currentWaypointIndex;

    public TrafficLight trafficLightController;

    public int stopWaypointIndex = -1; 

    private Transform stopWaypoint;


    void Start()
    {
        if (stopWaypointIndex >= 0 && stopWaypointIndex < waypoints.Count)
        {
            stopWaypoint = waypoints[stopWaypointIndex];
        }

        if (trafficLightController == null)
        {
            Debug.LogError("TrafficLight Controller not set on car " + gameObject.name);
        }
    }


    void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            return;
        }

        if (currentWaypointIndex == stopWaypointIndex)
        {
            if (CheckForStop())
            {
                return;
            }
        }

        MoveToWaypoint();
    }


    private bool CheckForStop()
    {
        float distanceToStopPoint = Vector3.Distance(transform.position, stopWaypoint.position);

        if (distanceToStopPoint <= minDistance * 2)
        {
            if (trafficLightController.currentPhase == TrafficLight.LightPhase.Red ||
                trafficLightController.currentPhase == TrafficLight.LightPhase.YellowToRed ||
                trafficLightController.currentPhase == TrafficLight.LightPhase.RedAndYellowToGreen)
            {
                return true;
            }
        }

        return false;
    }


    private void MoveToWaypoint()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        float distance = Vector3.Distance(transform.position, targetWaypoint.position);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWaypoint.position,
            moveSpeed * Time.deltaTime
        );

        RotateTowardsWaypoint(targetWaypoint);

        if (distance < minDistance)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= waypoints.Count)
            {
                currentWaypointIndex = 0; 
            }
        }
    }

    private void RotateTowardsWaypoint(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }
}