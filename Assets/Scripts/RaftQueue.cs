using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftQueue : MonoBehaviour
{
    [Header("References")]
    public Transform raftSelector;

    [Header("Spawning")]
    public GameObject raftPrefab;
    public Transform spawnPoint;
    public int maxRafts = 20;
    public float spawnInterval = 1.5f;

    [Header("Queue")]
    public int queueSize = 5;
    public float queueWidth = 10f;
    public Vector3 queueDirection = new Vector3(1, 0, 0);
    public float moveIntoQueueDuration = 0.75f;
    public Ease moveIntoQueueEase = Ease.OutCirc;

    private float nextRaftSpawnTime;

    private List<QueuedRaft> queuedRafts;

    // Start is called before the first frame update
    void Start()
    {
        queuedRafts = new List<QueuedRaft>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextRaftSpawnTime)
        {
            nextRaftSpawnTime = Time.time + spawnInterval;
            SpawnRaft();
        }
    }

    public Raft GetRandomRaft()
    {
        if (queuedRafts.Count > 0)
        {
            int randomIndex = Random.Range(0, queuedRafts.Count);

            Raft raft = queuedRafts[randomIndex].raft;

            RemoveRaftFromQueue(randomIndex);
            return raft;
        }
        else
        {
            return null;
        }       
    }

    private void SpawnRaft()
    {
        if (queuedRafts.Count < queueSize)
        {
            GameObject raft = Instantiate(raftPrefab);
            raft.gameObject.name = "Raft: " + Random.Range(0.0f, 1.0f);
            raft.transform.position = spawnPoint.position;

            QueuedRaft queuedRaft;
            queuedRaft.raft = raft.GetComponent<Raft>();
            queuedRaft.queueLocation = queuedRafts.Count;

            queuedRafts.Add(queuedRaft);

            OnQueueChanged();
        }
    }

    private void RemoveRaftFromQueue(int index)
    {
        queuedRafts.RemoveAt(index);
        OnQueueChanged();
    }

    private void OnQueueChanged()
    {
        for (int i = 0; i < queuedRafts.Count; i++)
        {
            QueuedRaft queuedRaft = queuedRafts[i];

            //reorder queue if necessary
            queuedRaft.queueLocation = i;

            //move all queued rafts to their proper positions
            Vector3 destinationPosition = GetQueueRaftPosition(queuedRaft);
            queuedRaft.raft.transform.DOMove(destinationPosition, moveIntoQueueDuration).SetEase(moveIntoQueueEase);
        }
    }

    private Vector3 GetQueueRaftPosition(QueuedRaft queuedRaft)
    {
        float stepSize = queueWidth / (queueSize - 1);

        return transform.position + (queueDirection * stepSize * queuedRaft.queueLocation) - queueDirection * queueWidth * 0.5f;
    }
}

public struct QueuedRaft
{
    public Raft raft;
    public int queueLocation;
}
