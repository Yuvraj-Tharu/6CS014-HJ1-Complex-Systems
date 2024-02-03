using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ACOTester : MonoBehaviour
{
    private ACOCON MyACOCON = new ACOCON();
    private List<GameObject> Waypoints = new List<GameObject>();
    private List<ACOConnection> Connections = new List<ACOConnection>();
    private List<ACOConnection> MyACORoute = new List<ACOConnection>();
    private Vector3 OffSet = new Vector3(0, 0.5f, 0);

    [SerializeField] private int MaxPathLength;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float currentSpeed = 50f;
    private int currentTargetIndex = 0;
    public bool agentMove = true;

    // private float range = 50;

    [SerializeField]
    private int carID;

    public int NumberOfParcel { get; private set; }
    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }

    public int GetCarID()
    {
        return carID;
    }

    private TextMeshProUGUI speedText;
    private TextMeshProUGUI parcelText;
    private TextMeshProUGUI sucessText;
    private TextMeshProUGUI collisionText;

    private PathfindingTester pt;

    [SerializeField] private GameObject StartNode;
    [SerializeField] private GameObject StopPoint1;
    [SerializeField] private GameObject StopPoint2;
    [SerializeField] private GameObject StopPoint3;

    public new BoxCollider collider;

    private float originalSpeed;
    // private ACOConnection lastConnection;


    void Start()
    {
        pt = GetComponent<PathfindingTester>();

        TextMeshProUGUI[] textContainers;
        textContainers = gameObject.GetComponentsInChildren<TextMeshProUGUI>();

        collider = GetComponent<BoxCollider>();

        foreach (TextMeshProUGUI g in textContainers)
        {
            if (g.name.Equals("speedTxt"))
            {
                speedText = g;

                speedText.text = "Speed: " + currentSpeed.ToString("F2") + " km/hr";
            }

            if (g.name.Equals("ParcelText"))
            {
                parcelText = g;
            }
            if (g.name.Equals("SucessText"))
            {
                sucessText = g;

            }
            if (g.name.Equals("CollisionText"))
            {
                collisionText = g;
            }
        }

        if (StartNode == null)
        {
            Debug.Log("No start waypoint node.");
            return;
        }

        VisGraphWaypointManager tmpWpM = StartNode.GetComponent<VisGraphWaypointManager>();
        if (tmpWpM == null)
        {
            Debug.Log("Start node is not a waypoint.");
            return;
        }

        GameObject[] GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            VisGraphWaypointManager tmpWaypointCon = waypoint.GetComponent<VisGraphWaypointManager>();
            // if (tmpWaypointCon && tmpWaypointCon.WaypointType == VisGraphWaypointManager.waypointPropsList.Goal)
            // {
            Waypoints.Add(waypoint);
            // }
        }




        foreach (GameObject waypoint in Waypoints)
        {
            VisGraphWaypointManager tmpWaypointCon = waypoint.GetComponent<VisGraphWaypointManager>();
            foreach (VisGraphConnection aVisGraphConnection in tmpWaypointCon.Connections)
            {
                ACOConnection aConnection = new ACOConnection();
                aConnection.SetConnection(waypoint, aVisGraphConnection.ToNode, MyACOCON.DefaultPheromone);
                Connections.Add(aConnection);
            }
        }

        if (Connections.Count <= 1)
        {
            Debug.Log("Warning, you have set 1 or 0 goal nodes. You need at least 2. However, more is expected.");
            return;
        }

        MyACORoute = MyACOCON.ACO(50, 25, Waypoints.ToArray(), Connections, StartNode, MaxPathLength);

        if (MyACORoute.Count == 0)
        {
            Debug.Log("Warning, ACO did not return a path. Please check all logs.");
        }
    }

    void OnDrawGizmos()
    {
        if (MyACORoute.Count > 0)
        {
            foreach (ACOConnection aConnection in MyACORoute)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet), (aConnection.ToNode.transform.position + OffSet));
            }
        }
    }

    void Update()
    {
        if (agentMove)
        {
            MoveAlongPath();
        }

        checkRadius();


    }


    void MoveAlongPath()
    {
        if (currentTargetIndex < MyACORoute.Count)
        {
            ACOConnection currentConnection = MyACORoute[currentTargetIndex];
            Vector3 targetPosition = currentConnection.ToNode.transform.position;

            // Calculate direction to the target
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            // Rotate towards the target direction
            if (direction.magnitude > 0.1f)
            {
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }


            // Move towards the target
            float step = currentSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Check if the path has changed


            // Check if close enough to the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentTargetIndex++;

                // Check if the current waypoint is the StopPoint
                if (currentConnection.ToNode == StopPoint1)
                {
                    agentMove = false;
                    Debug.Log("Goal point reached!");
                    StartCoroutine(StopForSeconds(2f));

                }

                if (currentConnection.ToNode == StopPoint2)
                {
                    agentMove = false;
                    Debug.Log("Goal point reached!");
                    StartCoroutine(StopForSeconds(2f));

                }

                if (currentConnection.ToNode == StopPoint3)
                {
                    agentMove = false;
                    Debug.Log("Goal point reached!");
                    if (pt != null)
                    {
                        pt.enabled = true;
                        Debug.Log("PathfindingTester enabled.");
                    }
                    enabled = false;
                }



            }


        }

    }

    IEnumerator StopForSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        // Resume movement after waiting

        agentMove = true;

        // Update speed text in UI
    }




    public void CollectParcel()
    {
        NumberOfParcel++;
        currentSpeed *= 0.9f;

    }



    private void OnTriggerEnter(Collider other)
    {
        Parcel parcel = other.gameObject.GetComponent<Parcel>();
        Debug.Log("OnTriggerEnter called");

        if (parcel != null)
        {
            int boxId = parcel.GetId();

            if (boxId == carID)
            {
                CollectParcel();
                other.gameObject.SetActive(false);
                // StartCoroutine(DeactivateParcel(other.gameObject, 1.0f));

                StartCoroutine(ParcelCollectMessage());

                originalSpeed = currentSpeed;


            }

        }


        speedText.text = "Speed: " + currentSpeed.ToString("F2") + " km/hr";
        parcelText.text = NumberOfParcel.ToString();

    }





    void checkRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(collider.transform.position, 30, LayerMask.GetMask("Car"));

        if (colliders.Length > 1)
        {
            foreach (Collider col in colliders)
            {
                if (col.gameObject != gameObject)
                {
                    ACOTester agent = col.GetComponent<ACOTester>();
                    float distance = Vector3.Distance(col.transform.position, transform.position);
                    float speed2 = agent.CurrentSpeed;

                    if (distance < 20 && currentSpeed < speed2)
                    {
                        agentMove = false;
                        StartCoroutine(CollisionMessage());
                    }
                    else
                    {

                        agentMove = true;
                    }
                }
                // else
                // {
                //     Debug.Log("Here");
                //     agentMove = true;
                // }
            }
        }

    }

    // IEnumerator ResumeNormalSpeedAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);

    //     // Resume normal movement after waiting
    //     agentMove = true;
    // }








    private IEnumerator ParcelCollectMessage()
    {
        sucessText.text = "Parcel Collected Successfully";
        yield return new WaitForSeconds(1.0f);
        sucessText.text = "";
    }

    private IEnumerator CollisionMessage()
    {

        Debug.Log("Collision Message Coroutine Started");
        collisionText.text = "Collision Occur !!";
        yield return new WaitForSeconds(2.0f);
        Debug.Log("Collision Message Coroutine Ended");
        collisionText.text = "";
    }


}