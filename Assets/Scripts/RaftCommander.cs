using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftCommander : MonoBehaviour
{
    [Header("References")]
    public RaftQueue raftQueue;
    public Transform raftPlacementSelector;

    [Header("Raft Detection")]
    public LayerMask raftCheckMask;
    public float visiblityRadius = 4f;
    public float checkInterval = 1.0f;

    private Raft currentRaft;
    private ConnectionDirection currentDirection;

    private List<Raft> visibleRafts;

    private RaycastHit[] raftCheckHits;
    private Collider[] visibleRaftCheckColliders;

    private float nextCheckRaftsTime = 0;

    private Vector3 NETHERREALMPOSITION = new Vector3(100, 100, 100); 

    // Start is called before the first frame update
    void Start()
    {
        raftCheckHits = new RaycastHit[5];
        visibleRaftCheckColliders = new Collider[15];
        visibleRafts = new List<Raft>();
}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            HailRaft();
        }
    }

    private void FixedUpdate()
    {
        if (Time.time > nextCheckRaftsTime)
        {
            nextCheckRaftsTime = Time.time + checkInterval;
            CheckForCurrentRaft();
            CalculateRaftDirection();

            
            //FindAllVisibleRafts();
        }

        DisplayRaftPlacement();
    }

    private void HailRaft()
    {
        Raft closestRaft = raftQueue.GetSelectedRaft();
        if (closestRaft != null)
        {
            closestRaft.JoinRaft(currentRaft, currentDirection);
        }
    }

    private Raft GetClosestRaft()
    {
        Raft raft = null;
        if (visibleRafts.Count > 0)
        {
            float shortestDist = float.MaxValue;
            for (int i = 0; i < visibleRafts.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, visibleRafts[i].transform.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    raft = visibleRafts[i];
                }
            }
        }

        return raft;
    }

    private void CheckForCurrentRaft()
    {
        if (Physics.RaycastNonAlloc(transform.position, Vector3.down, raftCheckHits, raftCheckMask) > 0)
        {
            for (int i = 0; i < raftCheckHits.Length; i++)
            {
                if (raftCheckHits[i].collider != null)
                {
                    Raft raft = null;
                    raft = raftCheckHits[i].collider.gameObject.GetComponentInParent<Raft>();

                    if (raft != null && !raft.Equals(currentRaft))
                    {
                        currentRaft = raft;
                    }
                }
            }
        }
    }

    private void CalculateRaftDirection()
    {
        if (currentRaft != null)
        {
            float shortestDist = float.MaxValue;

            float currentDist = Vector3.Distance(currentRaft.northConnection.position, transform.position);
            if (currentDist < shortestDist)
            {
                shortestDist = currentDist;
                currentDirection = ConnectionDirection.North;
            }

            currentDist = Vector3.Distance(currentRaft.southConnection.position, transform.position);
            if (currentDist < shortestDist)
            {
                shortestDist = currentDist;
                currentDirection = ConnectionDirection.South;
            }

            currentDist = Vector3.Distance(currentRaft.eastConnection.position, transform.position);
            if (currentDist < shortestDist)
            {
                shortestDist = currentDist;
                currentDirection = ConnectionDirection.East;
            }

            currentDist = Vector3.Distance(currentRaft.westConnection.position, transform.position);
            if (currentDist < shortestDist)
            {
                shortestDist = currentDist;
                currentDirection = ConnectionDirection.West;
            }
        }
    }

    private void DisplayRaftPlacement()
    {
        if (currentRaft == null)
        {
            return;
        }

        //check if there is only one wall on raft (meaning it is empty)
        if (currentRaft.IsRaftDirectionClear(currentDirection))
        {
            raftPlacementSelector.position = currentRaft.GetEmptyRaftPosition(currentDirection);
        }

    }
}
