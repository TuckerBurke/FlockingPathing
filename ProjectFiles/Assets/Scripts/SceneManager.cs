using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    // Global variables ********************************************************************************
    #region Variables and Properties

    public FlockManager flockManager;
    public GameObject[] trees;
    public GameObject[] nodes;
    public Camera[] cameras;
    public Material pathLines;
    public float obsticleRadius = 0.5f;
    public bool debugMode;
    public int currentCameraIndex;

    // Properties
    public GameObject[] Nodes { get { return nodes; } }

    #endregion


    // Use this for initialization *********************************************************************
    #region Start

    void Start ()
    {
        // Start with debug mode deactivated
        debugMode = false;

        // Obtain references
        trees = GameObject.FindGameObjectsWithTag("Obsticle");
        flockManager = GameObject.Find("FlockManager").GetComponent<FlockManager>();
        flockManager.debugPos.GetComponent<MeshRenderer>().enabled = false;

        // Manually lode nodes to force sort
        nodes = new GameObject[8];
        nodes[0] = GameObject.Find("Node");
        nodes[1] = GameObject.Find("Node (1)");
        nodes[2] = GameObject.Find("Node (2)");
        nodes[3] = GameObject.Find("Node (3)");
        nodes[4] = GameObject.Find("Node (4)");
        nodes[5] = GameObject.Find("Node (5)");
        nodes[6] = GameObject.Find("Node (6)");
        nodes[7] = GameObject.Find("Node (7)");

        // Manually load camera array to force sorting
        cameras = new Camera[4];
        cameras[0] = GameObject.Find("WholeScene").GetComponent<Camera>();
        cameras[1] = GameObject.Find("FlockBehind").GetComponent<Camera>();
        cameras[2] = GameObject.Find("FlockFront").GetComponent<Camera>();
        cameras[3] = GameObject.Find("PathFollow").GetComponent<Camera>();

        // Choose targets for applicable cameras
        cameras[1].GetComponent<SmoothFollow>().target = flockManager.debugPos.transform;
        cameras[2].GetComponent<SmoothFollow>().target = flockManager.debugPos.transform;
        cameras[3].GetComponent<SmoothFollow>().target = GameObject.Find("Cow").transform;

        // Initialize default camera
        currentCameraIndex = 0;

        // Turn off all other cameras
        for (int c = 1; c < cameras.Length; c++)
        {
            cameras[c].gameObject.SetActive(false);
        }

        // If additional cameras were added
        if (cameras.Length > 0)
        {
            // Enable the first one in array
            cameras[0].gameObject.SetActive(true);
        }
    }

    #endregion


    // Update ******************************************************************************************
    #region Update

    void Update ()
    {
        // Check for user input
        CheckForInput();
    }

    #endregion


    // User Input **************************************************************************************
    #region UserInput

    public void CheckForInput()
    {
        // If 'D' is pressed
        if (Input.GetKeyDown(KeyCode.D))
        {
            // If not in debug mode
            if (!debugMode)
            {
                // Activate debug mode
                debugMode = true;
                flockManager.debugMode = true;
                flockManager.debugPos.GetComponent<MeshRenderer>().enabled = true;
            }
            // If in debug mode
            else
            {
                //Deactivate debugMode
                debugMode = false;
                flockManager.debugMode = false;
                flockManager.debugPos.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        // If 'C' is pressed
        if(Input.GetKeyDown(KeyCode.C))
        {
            // Cycle to the next camera
            currentCameraIndex++;

            // If new index is valid, progress to next camera
            if(currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }

            // If index is out of range, return to default camera
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
        }
    }

    #endregion


    // Debug lines ************************************************************************************
    #region Debug Lines

    public void OnRenderObject()
    {
        // If in debug mode
        if (nodes[7] != null && debugMode)
        {
            // Draw line between each node
            pathLines.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(nodes[0].transform.position);
            GL.Vertex(nodes[1].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[1].transform.position);
            GL.Vertex(nodes[2].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[2].transform.position);
            GL.Vertex(nodes[3].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[3].transform.position);
            GL.Vertex(nodes[4].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[4].transform.position);
            GL.Vertex(nodes[5].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[5].transform.position);
            GL.Vertex(nodes[6].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[6].transform.position);
            GL.Vertex(nodes[7].transform.position);
            GL.End();

            GL.Begin(GL.LINES);
            GL.Vertex(nodes[7].transform.position);
            GL.Vertex(nodes[0].transform.position);
            GL.End();
        }
    }

    #endregion
}
