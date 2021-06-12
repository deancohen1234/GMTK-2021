using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftCommander : MonoBehaviour
{
    [Header("Raft Detection")]
    public LayerMask raftCheckMask;
    public float visiblityRadius = 4f;

    private Raft currentRaft;

    private List<Raft> visibleRafts;

    private RaycastHit[] raftCheckHits;
    private Collider[] visibleRaftCheckColliders;

    // Start is called before the first frame update
    void Start()
    {
        raftCheckHits = new RaycastHit[5];
        visibleRaftCheckColliders = new Collider[15];
        visibleRafts = new List<Raft>();
}

    // Update is called once per frame
    void Update()
    {
        CheckVisibleSize();
    }

    private void FixedUpdate()
    {
        CheckForCurrentRaft();

        //TODO put this on a timer
        FindAllVisibleRafts();
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

                    if (!visibleRafts.Contains(raft))
                    {
                        //Debug.Log("Adding: " + visibleRaftCheckColliders[i].name);
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
