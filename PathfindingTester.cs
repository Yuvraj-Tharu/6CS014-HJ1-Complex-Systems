

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class PathfindingTester : MonoBehaviour
{

    // The A* manager.
    private AStarManager AStarManager = new();
    // List of possible waypoints.
    private List<GameObject> Waypoints = new();
    // List of waypoint map connections. Represents a path.
    private List<Connection> ConnectionArray = new List<Connection>();
    // The start and end nodes.
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;

    // [SerializeField]
    // private int carID;


    Vector3 OffSet = new(0, 0.3f, 0);

    // [SerializeField]
    // private float currentSpeed;

    private int currentTarget = 0;
    private Vector3 currentTargetPos;
    private int moveDirection = 1;
    public bool agentMove = true;
    private float range = 50;
    private ACOTester acoTester;
    public new BoxCollider collider;



    void Start()
    {
        acoTester = GetComponent<ACOTester>();
        if (acoTester == null)
        {
            Debug.LogError("ACOTester script not found on the same GameObject.");
        }

        collider = GetComponent<BoxCollider>();

        if (start == null || end == null)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }
        VisGraphWaypointManager tmpWpM = start.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("Start is not a waypoint.");
            return;
        }
        tmpWpM = end.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("End is not a waypoint.");
            return;
        }

        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();
            if (tmpWaypointMan)
            {
                Waypoints.Add(waypoint);
            }
        }

        foreach (GameObject waypoint in Waypoints)
        {
            VisGraphWaypointManager tmpWaypointMan = waypoint.GetComponent<VisGraphWaypointManager>();

            foreach (VisGraphConnection aVisGraphConnection in tmpWaypointMan.Connections)
            {
                if (aVisGraphConnection.ToNode != null)
                {
                    Connection aConnection = new Connection();
                    aConnection.FromNode = waypoint;
                    aConnection.ToNode = aVisGraphConnection.ToNode;
                    AStarManager.AddConnection(aConnection);
                }
                else
                {
                    Debug.Log("Warning, " + waypoint.name + " has a missing to node for a connection!");
                }
            }
        }

        ConnectionArray = AStarManager.PathfindAStar(start, end);
        if (ConnectionArray.Count == 0)
        {
            Debug.Log("Warning, A* did not return a path between the start and end node.");
        }
    }

    void OnDrawGizmos()
    {
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet),
           (aConnection.ToNode.transform.position + OffSet));
        }
    }

    // Update is called once per frame
    void Update()
    {

        // if (agentMove)
        if (agentMove && ConnectionArray.Count > 0 && currentTarget >= 0 && currentTarget < ConnectionArray.Count)
        {

            if (moveDirection > 0)
            {
                currentTargetPos = ConnectionArray[currentTarget].ToNode.transform.position;

            }
            else
            {
                currentTargetPos = ConnectionArray[currentTarget].FromNode.transform.position;
            }

            currentTargetPos.y = transform.position.y;
            Vector3 direction = currentTargetPos - transform.position;
            float distance = direction.magnitude;
            direction.y = 0;

            if (direction.magnitude > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = rotation;
            }

            Vector3 normDirection = direction / distance;

            float currentSpeedFromACOTester = acoTester.CurrentSpeed;
            Debug.Log("------------------------------");
            Debug.Log("currentSpeedFromACOTester: " + currentSpeedFromACOTester);

            transform.position = transform.position + normDirection * currentSpeedFromACOTester * Time.deltaTime;
            Debug.Log("Current position: " + transform.position);

            if (distance < 1)
            {

                transform.position = currentTargetPos;

                if (currentTarget == ConnectionArray.Count - 1 && moveDirection > 0)
                {

                    Debug.Log("Agent reached the end point!");


                    // moveDirection *= -1;
                    agentMove = false;


                }
                //     else if (currentTarget == 0 && moveDirection < 0)
                //     {

                //         Debug.Log("Agent returned to the starting point!");
                //         transform.position = ConnectionArray[0].FromNode.transform.position;

                //         transform.rotation = Quaternion.identity;

                //         currentSpeed = 0;
                //         agentMove = false;
                //     }
                //     else
                //     {
                //         currentTarget += moveDirection;
                //     }


            }


        }




        // checkRadius();
    }


    // void checkRadius()
    // {



    //     RaycastHit hit;

    //     if (Physics.BoxCast(collider.bounds.center, transform.localScale * 0.5f, transform.forward, out hit, transform.rotation, range))
    //     {
    //         if (hit.collider.tag == "Car" && hit.collider.name != this.name)
    //         {
    //             PathfindingTester otherCar = hit.collider.gameObject.GetComponent<PathfindingTester>();

    //             if (otherCar != null && Vector3.Distance(otherCar.gameObject.transform.position, gameObject.transform.position) < 20)
    //             {
    //                 // float currentSpeedFromACOTester = acoTester.CurrentSpeed;
    //                 float otherCarSpeed = otherCar.currentSpeed;

    //                 if (currentSpeed <= otherCarSpeed)
    //                 {
    //                     agentMove = false;
    //                     // StartCoroutine(CollisionMessage());


    //                     return;
    //                 }
    //             }
    //         }
    //     }

    //     agentMove = true;


    // }


    // private IEnumerator CollisionMessage()
    // {

    //     Debug.Log("Collision Message Coroutine Started");
    //     collisionText.text = "Collision Occur !!";
    //     yield return new WaitForSeconds(2.0f);
    //     Debug.Log("Collision Message Coroutine Ended");
    //     collisionText.text = "";
    // }



}


