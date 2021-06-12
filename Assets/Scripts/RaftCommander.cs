using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftCommander : MonoBehaviour
{
    [Header("References")]
    public RaftQueue raftQueue;

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
            
            FindAllVisibleRafts();
        }       
    }

    private void HailRaft()
    {
        //Raft closestRaft = GetClosestRaft();
        Raft closestRaft = raftQueue.GetRandomRaft();
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

    private void FindAllVisibleRafts()
    {
        visibleRafts.Clear();

        //clear colliders
        for (int i = 0; i < visibleRaftCheckColliders.Length; i++)
        {
            visibleRaftCheckColliders[i] = null;
        }


        int foundRafts = Physics.OverlapSphereNonAlloc(transform.position, visiblityRadius, visibleRaftCheckColliders, raftCheckMask);
        if (foundRafts > 0)
        {
            for (int i = 0; i < visibleRaftCheckColliders.Length; i++)
            {
                if (visibleRaftCheckColliders[i] != null)
                {
                    Raft raft = null;
                    raft = visibleRaftCheckColliders[i].gameObject.GetComponentInParent<Raft>();

                    if (!visibleRafts.Contains(raft) && !raft.Equals(currentRaft))
                    {
                        visibleRafts.Add(raft);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < visibleRafts.Count; i++)
            {
                visibleRafts[i] = null;
            }
        }
    }

    private void CheckVisibleSize()
    {
        Debug.Log("Count: " + visibleRafts.Count);
    }
}
