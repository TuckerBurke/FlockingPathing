using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    // Variable and properties ***********************************************************************
    #region Variables

    // Prefabs
    public GameObject _sheep;
    public GameObject _averagePos;

    // Flock data
    public GameObject[] flock;
    public GameObject sheep;
    public GameObject debugPos;
    public int flockQuantity    = 10;
    public float spawnRadius    = 10.0f;
    public Vector3 averageDir;
    public Vector3 averagePos;

    // Force weights
    public float weightAlignment    = 1.0f;
    public float weightCohesion     = 1.0f;
    public float weightSeparation   = 1.0f;
    public float weightAvoidance    = 1.0f;

    // Debug stuff
    public bool debugMode;
    public Material averageDirection;

    // Properties
    public Vector3 AverageDir { get { return averageDir; } }
    public Vector3 AveragePos { get { return averagePos; } }
    public GameObject[] Flock { get { return flock; } }
    public GameObject DebugPos { get { return debugPos; } }
    public bool DebugMode { set { debugMode = value; } }

    #endregion


    // Use this for initialization *******************************************************************
    #region Start

    void Start ()
    {
        // Local variables
        float spawnPosX;
        float spawnPosY;
        float spawnPosZ;
        Vector3 spawnPos = new Vector3();
        float spawnAngle;
        Quaternion spawnRot;

        // Start with debug mode deactivated
        debugMode = false;

        // Spawn sheep
        // Instantiate array to correct size
        flock = new GameObject[flockQuantity];

        // Create sheep for each slot in array
        for(int s = 0; s < flockQuantity; s++)
        {
            // Generate random spawn location within spawn radius
            spawnPosX = Random.Range(-spawnRadius, spawnRadius);
            spawnPosZ = Random.Range(-spawnRadius, spawnRadius);
            spawnPos.x = spawnPosX;
            spawnPos.z = spawnPosZ;
            spawnPosY = Terrain.activeTerrain.SampleHeight(spawnPos);
            spawnPos.y = spawnPosY;

            // Generate random rotation +/- 70 degrees
            spawnAngle = Random.Range(-70f, 70f);
            spawnRot = Quaternion.Euler(0, spawnAngle, 0);            

            // Instantiate sheep in flock
            flock[s] = Instantiate(_sheep, spawnPos, spawnRot);
        }

        // Instantiate debug object
        debugPos = Instantiate(_averagePos);       
    }

    #endregion


    // Update is called once per frame ***************************************************************
    #region Update

    void Update ()
    {
        // Calculate averages
        AveragePosition();
        AverageDirection();
	}

    #endregion


    // Calculations for average position and direction ************************************************
    #region Averages

    void AveragePosition ()
    {
        // Reset variables
        averagePos = Vector3.zero;

        // For each sheep in flock
        for(int s = 0; s < flockQuantity; s++)
        {
            // Sum positions
            averagePos += flock[s].transform.position;
        }

        // Calculate average
        averagePos /= flockQuantity;
        averagePos.y = 1.5f;

        // Apply to debug object
        debugPos.transform.position = averagePos;
    }


    void AverageDirection ()
    {
        // Reset variables
        averageDir = Vector3.zero;

        // For each sheep in flock
        for(int s = 0; s < flockQuantity; s++)
        {
            // Sum directions
            averageDir += flock[s].transform.forward;
        }

        // Calculate average
        averageDir.Normalize();
        averageDir.y = 0;

        //Apply to debug object
        debugPos.transform.forward = averageDir;
    }

    #endregion


    // Debug Lines ************************************************************************************
    #region Debug Lines

    public void OnRenderObject()
    {
        // If in debug mode
        if (debugPos != null && debugMode)
        {
            // Draw line for flocks average direction
            averageDirection.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(debugPos.transform.position);
            GL.Vertex(debugPos.transform.position + averageDir);
            GL.End();
        }
    }

    #endregion
}
