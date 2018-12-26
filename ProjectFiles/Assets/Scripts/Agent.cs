using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    // Variables *************************************************************************************
    #region Variables

    public SceneManager sceneManager;
    public Vector3 position;
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 steeringForce;
    public Vector3 wanderHandle;
    public float wanderHandleLength = 2.2f;
    public float mass           = 1f;
    public float maxSpeed       = 5f;
    public float maxForce       = 10f;
    public float wanderAngle    = 0f;
    public float agentRadius    = 0.4f;
    public float boundaryRadius;
    public float obsticleRadius;

    #endregion


    // Use this for initialization *******************************************************************
    #region Start

    protected virtual void Start ()
    {
        // Initialize instance variables
        position = transform.position;
        direction = transform.forward;
        boundaryRadius = GameObject.Find("FlockManager").GetComponent<FlockManager>().spawnRadius;
        sceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        obsticleRadius = GameObject.Find("SceneManager").GetComponent<SceneManager>().obsticleRadius;
    }

    #endregion


    // Calculate forces ******************************************************************************
    #region Forces

    protected abstract void CalcSteeringForce();


    public void ApplyForce(Vector3 force)
    {
        // Apply Newton's 2nd law
        acceleration += force / mass;
    }


    public void Boundary ()
    {
        // Local variables
        float distanceFromCenter;

        // Calulate distance from center
        distanceFromCenter = (Vector3.zero - position).magnitude;

        // If the distance is greater than boundary radius
        if(distanceFromCenter > boundaryRadius)
        {
            // Apply steering force
            steeringForce += (Vector3.zero - position).normalized * maxSpeed;
        }
    }


    public Vector3 Seek(Vector3 target)
    {
        // Local variables
        Vector3 directionToTarget;
        Vector3 seekForce;

        // Calculate targeting
        directionToTarget = target - position;
        directionToTarget = directionToTarget.normalized;

        // Calculate seek force
        seekForce = (directionToTarget * maxSpeed) - velocity;

        return seekForce;
    }


    public Vector3 Flee(Vector3 predator)
    {
        // Local variables
        Vector3 directionFromPredator;
        Vector3 fleeForce;

        // Calculate targeting
        directionFromPredator = position - predator;
        directionFromPredator = directionFromPredator.normalized;

        // Calculate flee force
        fleeForce = (directionFromPredator * maxSpeed) - velocity;

        return fleeForce;
    }


    public void Wander ()
    {
        // Local variables
        Vector3 difference;

        // Create wander handle
        wanderHandle = position + (transform.forward * wanderHandleLength);

        // Accumulate random angling for wandering
        wanderAngle += Random.Range(-10f, 10f);

        // Correct accumulated angles if obtuse
        if(wanderAngle > 90)
        {
            wanderAngle -= 180;
        }
        else if(wanderAngle < -90)
        {
            wanderAngle = 180 - wanderAngle;
        }

        // Rotate handle based on angle
        wanderHandle += Quaternion.Euler(0, wanderAngle, 0) * transform.forward;

        // Calculate difference in direction
        difference = wanderHandle - position;
        difference = difference.normalized;

        // Apply to forces
        steeringForce += (difference * maxSpeed) - velocity;
    }

    #endregion


    // Calculate agent movement **********************************************************************
    #region Movement

    public void Movement ()
    {
        // Calculate velocity based on acceleration
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Calculate new position based on velocity
        position += velocity * Time.deltaTime;
        direction = velocity.normalized;

        // Reset variables
        acceleration = Vector3.zero;
    }


    public void SetTransform ()
    {
        // Update variables using terrain height
        //position = transform.position;
        position.y = Terrain.activeTerrain.SampleHeight(position);

        // Apply transformation
        transform.position = position;
        transform.forward = direction;
    }

    #endregion
}
