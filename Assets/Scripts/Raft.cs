using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//North = Z Forward (Global) +
//South = Z Backward (Global) -
//East = X Forward (Global) +
//West = X Backward (Global) -

public enum ConnectionDirection { North, South, East, West }
public class Raft : MonoBehaviour
{
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

    [Header("Connection Sequence")]
    public float dockingDuration = 2.0f;
    public Ease easeType = Ease.InOutCubic;

    private Vector3 northConnectionOffset;
    private Vector3 southConnectionOffset;
    private Vector3 eastConnectionOffset;
    private Vector3 westConnectionOffset;

    private bool isConnectedToPlatform;

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

        PrecalculateDirectionOffsets();
    }

    //join THIS raft to the destination raft at the set direction
    public void JoinRaft(Raft destinationRaft, ConnectionDirection direction)
    {
        if (destinationRaft == null)
        {
            Debug.LogError("Oi!! Calling Raft is null!");
            return;
        }

        Vector3 destinationRaftPosition = GetRaftConnectionPosition(destinationRaft, direction);
        Debug.Log("Pos: " + destinationRaftPosition);

        transform.DOMove(destinationRaftPosition, dockingDuration).SetEase(easeType);
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

    public bool IsConnectedToPlatform()
    {
        return isConnectedToPlatform;
    }

    //direction is the connection direction that THIS raft is using to connect to the platform
    private Vector3 GetRaftConnectionPosition(Raft raft, ConnectionDirection direction)
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

    private void PrecalculateDirectionOffsets()
    {
        northConnectionOffset = (transform.position - northConnection.position);
        southConnectionOffset = (transform.position - southConnection.position);
        eastConnectionOffset = (transform.position - eastConnection.position);
        westConnectionOffset = (transform.position - westConnection.position);
    }
}
