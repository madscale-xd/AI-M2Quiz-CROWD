using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Vector3 direction = Vector3.forward; // Movement Direction
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public Transform targetLocation;

    private void Update()
    {
        // Calculate the direction to the goal
        Vector3 targetDirection = (targetLocation.position - transform.position).normalized;
        targetDirection.y = 0.0f; 

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {
            
            Vector3 avoidDirection = Vector3.Cross(Vector3.up, hit.normal);
            avoidDirection.y = 0.0f;

            Debug.DrawLine(transform.position, hit.point, Color.red); // Draw a red line to visualize the raycast hit point
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green); // Draw a green line to visualize the hit normal

            targetDirection = Vector3.Lerp(targetDirection, avoidDirection, Mathf.Clamp01(hit.distance / 20f));
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 20f, Color.blue); // Draw a blue line to visualize the raycast
        }

        transform.Translate(targetDirection * speed * Time.deltaTime, Space.World);

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }


}
