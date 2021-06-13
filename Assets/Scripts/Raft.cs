using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

//North = Z Forward (Global) +
//South = Z Backward (Global) -
//East = X Forward (Global) +
//West = X Backward (Global) -

public enum ConnectionDirection { North, South, East, West }
public enum RaftType { Lane, Libations, Lodging, Larder}
public class Raft : MonoBehaviour
{
    [Header("Setup")]
    public RaftType raftType;
    public string onConnectedTriggerString;

    [Header("Connection Points")]
    public Transform northConnection;
    public Transform southConnection;
    public Transform eastConnection;
    public Transform westConnection;

    [Header("Colliders")]
    public Collider northCollider;
    public Collider southCollider;
    public Collider eastCollider;
    public Collider westCollider;
    public Collider groundCollider;

    [Header("Extras")]
    public GameObject[] allProps;
    public Material[] allMaterials;
    public MeshRenderer meshRenderer;

    [Header("Connection Sequence")]
    public int dockedLayer = 8;
    public float dockingDuration = 2.0f;
    public float dockingXOffset = 2.5f;
    public float dockingAccleration = 20f;
    public Ease easeType = Ease.InOutCubic;

    private Vector3 northConnectionOffset;
    private Vector3 southConnectionOffset;
    private Vector3 eastConnectionOffset;
    private Vector3 westConnectionOffset;

    private List<Raft> connectedRafts;
    private Collider[] wallCheckColliders;

    private BezierCurve moveToPlatformCurve;
    private Raft lastRaft;

    private bool isConnectingToPlatform;
    private bool fullySatisfied; //if it has a lodge, a larder, and a bar
    private float curveTime;

    private const float WALLSPHERECHECKRADIUS = 0.25f;

    void Start()
    {
        if (!northCollider || !southCollider || !eastCollider || !westCollider)
        {
            Debug.LogError("No Collider Set For Raft damnit");
            return;
        }

        if (!northConnection || !southConnection || !eastConnection || !westConnection)
        {
            Debug.LogError("No Collider Set For Connections damnit");
            return;
        }

        wallCheckColliders = new Collider[5];
        connectedRafts = new List<Raft>();

        PrecalculateDirectionOffsets();
    }

    private void Update()
    {
        if (isConnectingToPlatform)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveToPlatformCurve.Evaluate(curveTime), Time.deltaTime * dockingAccleration);
        }
    }

    public void Initialize()
    {
        //randomize props and mats
        //if > 0.5f then show prop
        for (int i = 0; i < allProps.Length; i++)
        {
            float rand = Random.Range(0.0f, 1.0f);

            if (rand < 0.5f) { allProps[i].SetActive(false); }
        }

        int randomMaterialIndex = Random.Range(0, allMaterials.Length);
        meshRenderer.material = allMaterials[randomMaterialIndex];
    }

    //join THIS raft to the destination raft at the set direction
    public void JoinRaft(Raft destinationRaft, ConnectionDirection direction)
    {
        if (destinationRaft == null)
        {
            Debug.LogError("Oi!! Calling Raft is null!");
            return;
        }

        StartRaftMoveSequence(destinationRaft, direction);

        SetAllToLayer(dockedLayer);
    }
    private void StartRaftMoveSequence(Raft destinationRaft, ConnectionDirection direction)
    {
        Vector3 destinationRaftPosition = GetRaftConnectionPosition(destinationRaft, direction);
        Vector3 anchorPosition = GetRaftBezierAnchorPoint(destinationRaftPosition);

        moveToPlatformCurve = new BezierCurve(transform.position, destinationRaftPosition, anchorPosition);

        DOTween.To(() => curveTime, x => curveTime = x, 1.0f, dockingDuration).SetEase(easeType).OnComplete(OnJoinComplete);

        lastRaft = destinationRaft;

        isConnectingToPlatform = true;
    }

    private void OnJoinComplete()
    {
        isConnectingToPlatform = false;
        curveTime = 0;

        EvaulateCollisionWalls();
        CalculatePoints();
    }

    private void EvaulateCollisionWalls()
    {
        if (lastRaft == null) { return; }

        //do an overlap boxtest with each wall to evaluate walls
        int count = 0;

        count = Physics.OverlapSphereNonAlloc(northConnection.position + Vector3.up * 0.5f, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
        if (count >= 2) { ProcessConnectedRaft(count); }

        count = Physics.OverlapSphereNonAlloc(southConnection.position + Vector3.up * 0.5f, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
        if (count >= 2) { ProcessConnectedRaft(count); }

        count = Physics.OverlapSphereNonAlloc(eastConnection.position + Vector3.up * 0.5f, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
        if (count >= 2) { ProcessConnectedRaft(count); ; }

        count = Physics.OverlapSphereNonAlloc(westConnection.position + Vector3.up * 0.5f, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
        if (count >= 2) { ProcessConnectedRaft(count); }
    }

    private void CalculatePoints()
    {
        int pointDelta = 1; //always start with one point

        bool isFullySatisfied = IsRaftFullySatisfied();

        pointDelta += isFullySatisfied ? 5 : 0;

        GameManager.instance.AddPoints(pointDelta);
    }


    //used saved buffer to get connecte raft
    private void ProcessConnectedRaft(int count)
    {
        for (int i = 0; i < wallCheckColliders.Length; i++)
        {
            Raft raft = GetRaftFromCollider(wallCheckColliders[i]);
            if (raft != null && !raft.Equals(this))
            {
                connectedRafts.Add(raft);
            }
        }

        if (raftType == RaftType.Lane)
        {
            DisableCurrentWallColliders(count);
        }
    }

    public Transform GetConnectionTransform(ConnectionDirection direction)
    {
        switch (direction)
        {
            case ConnectionDirection.North:
                return northConnection;
            case ConnectionDirection.South:
                return southConnection;
            case ConnectionDirection.East:
                return eastConnection;
            case ConnectionDirection.West:
                return westConnection;
            default:
                return null;
        }
    }

    public Raft GetRaftFromCollider(Collider collider)
    {
        if (collider != null)
        {
            return collider.GetComponentInParent<Raft>();
        }
        else
        {
            return null;
        }
    }

    public Vector3 GetEmptyRaftPosition(ConnectionDirection direction)
    {
        switch (direction)
        {
            case ConnectionDirection.North:
                return northConnection.position - northConnectionOffset;
            case ConnectionDirection.South:
                return southConnection.position - southConnectionOffset;
            case ConnectionDirection.East:
                return eastConnection.position - eastConnectionOffset;
            case ConnectionDirection.West:
                return westConnection.position - westConnectionOffset;
            default:
                return Vector3.zero;
        }
    }

    public bool IsRaftDirectionClear(ConnectionDirection direction)
    {
        int count = 0;

        switch (direction)
        {
            case ConnectionDirection.North:
                count = Physics.OverlapSphereNonAlloc(this.northConnection.position, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
                break;
            case ConnectionDirection.South:
                count = Physics.OverlapSphereNonAlloc(this.southConnection.position, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
                break;
            case ConnectionDirection.East:
                count = Physics.OverlapSphereNonAlloc(this.eastConnection.position, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
                break;
            case ConnectionDirection.West:
                count = Physics.OverlapSphereNonAlloc(this.westConnection.position, WALLSPHERECHECKRADIUS, wallCheckColliders, (1 << dockedLayer));
                break;
        }

        return count <= 1;
    }

    //direction is the connection direction that THIS raft is using to connect to the platform
    public Vector3 GetRaftConnectionPosition(Raft raft, ConnectionDirection direction)
    {
        Vector3 raftConnectionPosition = raft.GetConnectionTransform(direction).position;

        switch (direction)
        {
            case ConnectionDirection.North:
                return raftConnectionPosition - northConnectionOffset;
            case ConnectionDirection.South:
                return raftConnectionPosition - southConnectionOffset;
            case ConnectionDirection.East:
                return raftConnectionPosition - eastConnectionOffset;
            case ConnectionDirection.West:
                return raftConnectionPosition - westConnectionOffset;
            default:
                return raftConnectionPosition;
        }
    }

    private Vector3 GetRaftBezierAnchorPoint(Vector3 destinationPoint)
    {
        float sign = destinationPoint.x <= 0 ? -1f : 1f;

        Vector3 anchorPosition = new Vector3(dockingXOffset * sign + transform.position.x, transform.position.y, destinationPoint.z);
        return anchorPosition;
    }

    //if there is a lodge that connected that isn't already satisfied, and it now has all it's pieces
    private bool IsRaftFullySatisfied()
    {
        if (raftType == RaftType.Lodging)
        {
            return IsLodgingRaftComplete(this);
        }
        else
        {
            for (int i = 0; i < connectedRafts.Count; i++)
            {
                //only satisfied when lodging is present
                if (connectedRafts[i].raftType == RaftType.Lodging)
                {
                    return IsLodgingRaftComplete(connectedRafts[i]);
                }
            }
        }
        return false;
    }

    private bool IsLodgingRaftComplete(Raft lodgeRaft)
    {
        short completeMask = 0;
        for (int j = 0; j < lodgeRaft.connectedRafts.Count; j++)
        {
            if (lodgeRaft.connectedRafts[j].raftType == RaftType.Larder)
            {
                completeMask = 1 | 1;
            }

            if (lodgeRaft.connectedRafts[j].raftType == RaftType.Libations)
            {
                completeMask = 1 | 2;
            }
        }

        //= 0011 = 3. So if there is a 3 then both raft types are there
        if ((completeMask & 3) == completeMask)
        {
            return true;
        }

        return false;
    }

    private void PrecalculateDirectionOffsets()
    {
        northConnectionOffset = (transform.position - northConnection.position);
        southConnectionOffset = (transform.position - southConnection.position);
        eastConnectionOffset = (transform.position - eastConnection.position);
        westConnectionOffset = (transform.position - westConnection.position);
    }

    private void SetAllToLayer(int layer)
    {
        gameObject.layer = layer;

        northCollider.gameObject.layer = layer;
        southCollider.gameObject.layer = layer;
        eastCollider.gameObject.layer = layer;
        westCollider.gameObject.layer = layer;
        groundCollider.gameObject.layer = layer;

        northConnection.gameObject.layer = layer;
        southConnection.gameObject.layer = layer;
        eastConnection.gameObject.layer = layer;
        westConnection.gameObject.layer = layer;
    }

    private void DisableCurrentWallColliders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (wallCheckColliders[i] != null)
            {
                wallCheckColliders[i].enabled = false;
            }
        }
    }
}
