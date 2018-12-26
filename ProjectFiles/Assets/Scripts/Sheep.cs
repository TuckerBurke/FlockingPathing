using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Agent
{
    // Variables ******************************************************************************
    #region Variables

    public FlockManager flockManager;
    public GameObject[] flock;
    public GameObject[] trees;
    public Vector3 separationForce;
    public Vector3 treeToSheep;
    public float separationRadius = 1.5f;
    public float dangerZone = 4f;

    #endregion


    // Use this for initialization ************************************************************
    #region Start

    protected override void Start ()
    {
        // Call start from Agent class
        base.Start();

        // Initialize additional variables
        flockManager = GameObject.Find("FlockManager").GetComponent<FlockManager>();
        flock = flockManager.flock;
        separationForce = Vector3.zero;
        trees = sceneManager.trees;
    }

    #endregion


    // Update is called once per frame ********************************************************
    #region Update

    void Update()
    {
        // Reset variables
        position = transform.position;
        steeringForce = Vector3.zero;

        // Calculate forces
        CalcSteeringForce();
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        Boundary();

        // Positioning
        ApplyForce(steeringForce);
        Movement();
        SetTransform();
    }

    #endregion


    // Calculate sheep specific forces *********************************************************
    #region Forces

    protected override void CalcSteeringForce()
    {
        // Call sheep specific behaviors
        Wander();
        Flock();
        steeringForce += Avoidance() * flockManager.weightAvoidance;
    }


    public void Flock ()
    {
        // Call the 3 elements of flocking
        steeringForce += Alignment() * flockManager.weightAlignment;
        steeringForce += Cohesion() * flockManager.weightCohesion;
        steeringForce += Separation() * flockManager.weightSeparation;
    }


    public Vector3 Alignment ()
    {
        // Scale flocks average direction
        Vector3 maxFlockVelocity = flockManager.averageDir * maxSpeed;

        // Calculate desired velocity
        Vector3 desiredVelocity = maxFlockVelocity - velocity;

        return desiredVelocity;
    }


    public Vector3 Cohesion ()
    {
        // seek centroid
        return Seek(flockManager.averagePos);
    }


    public Vector3 Separation ()
    {
        // Reset variables
        separationForce = Vector3.zero;

        // For each sheep in the flock
        for(int s = 0; s < flock.Length; s++)
        {
            // So long as its not itself
            if(flock[s] != this)
            {
                // If within separation radius
                if((flock[s].transform.position - position).magnitude < separationRadius)
                {
                    // Flee the nearby sheep
                    separationForce += Flee(flock[s].transform.position);
                }
            }
        }

        return separationForce;
    }



    #endregion


    // Obstacle avoidance ************************************************************************
    #region Avoidance

    public Vector3 Avoidance()
    {
        // For each obsticle in the scene
        for(int t = 0; t < trees.Length; t++)
        {
            // Create vector between tree and sheep
            treeToSheep = trees[t].transform.position - position;
            treeToSheep.y = 0;

            // If it is within the danger zone and is in front of this sheep
            if(treeToSheep.magnitude < dangerZone && Vector3.Dot(treeToSheep, transform.forward) > 0)
            {
                // Determine if the radii of sheep and obsticle will intersect
                // *** if magnetude of projection is less than sum of radii
                if(Mathf.Abs(Vector3.Dot(treeToSheep, transform.right)) <= obsticleRadius + agentRadius)
                {
                    // Determine if agent should move to the left or the right
                    if(Vector3.Dot(treeToSheep, transform.right) >= 0)
                    {
                        // Move to the left
                        // Calculate desired velocity
                        Vector3 desiredVelocity = (-transform.right * maxSpeed) - velocity;

                        // Scale proportional to distance
                        desiredVelocity *= velocity.magnitude / treeToSheep.magnitude;

                        return desiredVelocity;
                    }
                    else
                    {
                        // Move to the right
                        // Calculate desired velocity
                        Vector3 desiredVelocity = (transform.right * maxSpeed) - velocity;

                        // Scale proportional to distance
                        desiredVelocity *= velocity.magnitude / treeToSheep.magnitude;

                        return desiredVelocity;
                    }
                }
            }
        }

        // No obsticle, no steering required
        return Vector3.zero;
    }

    #endregion
}
