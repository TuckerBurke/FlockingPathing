using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Agent
{
    // Variables *************************************************************************
    #region Variables

    public GameObject[] nodes;
    public GameObject targetNode;
    public Vector3 distanceToNode;
    public int nodeCount;
    
    #endregion


    // Use this for initialization ******************************************************
    #region Start

    protected override void Start ()
    {
        // Call start from Agent base class
        base.Start();

        // Obtain reference to nodes
        nodes = sceneManager.nodes;
        nodeCount = 0;
        targetNode = nodes[nodeCount];
    }

    #endregion


    // Update is called once per frame **************************************************
    #region Update

    void Update()
    {
        // Reset variables
        position = transform.position;
        steeringForce = Vector3.zero;

        // Calculate forces
        CalcSteeringForce();
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

        // Positioning
        ApplyForce(steeringForce);
        Movement();
        SetTransform();
    }

    #endregion


    // Calculate autonomous forces ******************************************************
    #region Forces

    protected override void CalcSteeringForce()
    {
        // Call sheep specific behaviors
        Wander();
        steeringForce += FollowPath() * 2;
    }

    public Vector3 FollowPath ()
    {
        // Calculate distance to target node
        distanceToNode = targetNode.transform.position - position;

        // If close to node
        if(distanceToNode.magnitude < 2.5f)
        {
            // If there is a next node in array
            if(nodeCount < 7)
            {
                // Target the next node
                nodeCount++;
                targetNode = nodes[nodeCount];
            }
            // Otherwise target first node
            else
            {
                nodeCount = 0;
                targetNode = nodes[nodeCount];
            }

            // Seek target node
            return Seek(targetNode.transform.position);
        }

        // Otherwise seek current target
        return Seek(targetNode.transform.position);
    }

    #endregion
}
