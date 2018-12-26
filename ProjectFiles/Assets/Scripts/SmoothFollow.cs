using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    // Variables **************************************************************************************
    #region Variables

    public Transform target;
    public float distance = 3.0f;
    public float height = 1.50f;
    public float heightDamping = 2.0f;
    public float positionDamping = 2.0f;
    public float rotationDamping = 2.0f;

    #endregion


    // Update is called once per frame *****************************************************************
    #region LateUpdate

    void LateUpdate()
    {
        // Early exit if there’s no target
        if (!target) return;

        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Set the position of the camera 
        Vector3 wantedPosition = target.position - target.forward * distance;
        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * positionDamping);

        // Adjust the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // If flock behind
        if (transform.name == "FlockBehind")
        {
            // Look forward
            transform.forward = Vector3.Lerp(transform.forward, target.forward, Time.deltaTime * rotationDamping);
        }

        // If flock behind
        else if (transform.name == "FlockFront")
        {
            // Look backwards
            transform.forward = Vector3.Lerp(transform.forward, target.forward * -1, Time.deltaTime * rotationDamping);
        }

        // If path follow
        else if (transform.name == "FlockFront")
        {
            // Look downwards
            transform.forward = Vector3.Lerp(transform.forward, target.up * -1, Time.deltaTime * rotationDamping);
        }
    }

    #endregion

}
