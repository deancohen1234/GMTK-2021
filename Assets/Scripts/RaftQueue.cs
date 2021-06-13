using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftQueue : MonoBehaviour
{
    [Header("References")]
    public Transform raftSelector;

    [Header("Spawning")]
    public GameObject lanePrefab;
    public GameObject lodgePrefab;
    public GameObject larderPrefab;
    public GameObject libationPrefab;
    public Transform spawnPoint;
    public int maxRafts = 20;
    public float spawnInterval = 1.5f;
    public Vector4 weightedOdds = new Vector4(5, 1, 1, 1);

    [Header("Queue")]
    public int queueSize = 5;
    public float queueWidth = 10f;
    public Vector3 queueDirection = new Vector3(1, 0, 0);
    public float moveIntoQueueDuration = 0.75f;
    public Ease moveIntoQueueEase = Ease.OutCirc;

    [Header("Ditching")]
    public Vector2 waitInQueueDurationMinMax = new Vector2(3f, 4f);
    public float ditchMoveDuration = 2f;
    public Ease ditchEase = Ease.InBack;

    private float nextRaftSpawnTime;

    private List<QueuedRaft> queuedRafts;
    private GameObject[] raftTypes;
    private int[] weightedIndexArray;

    private int currentSelection;
    private const float SELECTORHEIGHT = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        queuedRafts = new List<QueuedRaft>();

        raftTypes = new GameObject[] { lanePrefab, lodgePrefab, larderPrefab, libationPrefab};

        UpdateSelection();
        GenerateWeightedIndexArray();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        CheckRaftQueueTimes();

        if (Time.time > nextRaftSpawnTime)
        {
            nextRaftSpawnTime = Time.time + spawnInterval;
            SpawnRaft();
        }
    }

    //I HATE THIS I'M SORRY
    private void GenerateWeightedIndexArray()
    {
        weightedIndexArray = new int[(int)(weightedOdds.x + weightedOdds.y + weightedOdds.z + weightedOdds.w)];

        for (int i = 0; i < (int)weightedOdds.x; i++)
        {
            weightedIndexArray[i] = 0;
        }

        for (int i = (int)weightedOdds.x; i < (int)weightedOdds.x + (int)weightedOdds.y; i++)
        {
            weightedIndexArray[i] = 1;
        }

        for (int i = (int)weightedOdds.x + (int)weightedOdds.y; i < (int)weightedOdds.x + (int)weightedOdds.y + +(int)weightedOdds.z; i++)
        {
            weightedIndexArray[i] = 2;
        }

        for (int i = (int)weightedOdds.x + (int)weightedOdds.y + +(int)weightedOdds.z; i < (int)weightedOdds.x + (int)weightedOdds.y + (int)weightedOdds.z + (int)weightedOdds.w; i++)
        {
            weightedIndexArray[i] = 3;
        }
    }

    private void GetInputs()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //move selection to left
            currentSelection = (int)Mathf.Repeat(currentSelection - 1f, queueSize);
            UpdateSelection();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            //move selection to right
            currentSelection = (int)Mathf.Repeat(currentSelection + 1f, queueSize);
            UpdateSelection();
        }
    }

    private void CheckRaftQueueTimes()
    {
        for (int i = 0; i < queuedRafts.Count; i++)
        {
            if (Time.time > queuedRafts[i].ditchQueueTime)
            {
                DitchBoat(i);
            }
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

    public Raft GetSelectedRaft()
    {
        if (queuedRafts.Count > 0)
        {
            QueuedRaft queuedRaft = queuedRafts.Find(r => r.queueLocation == currentSelection);

            if (queuedRaft.raft == null) { return null; }

            int indexFromSelection = queuedRafts.IndexOf(queuedRaft);
            Raft raft = queuedRafts[indexFromSelection].raft;

            RemoveRaftFromQueue(indexFromSelection);
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
            GameObject raft = Instantiate(PickRaftType());
            raft.gameObject.name = "Raft: " + Random.Range(0.0f, 1.0f);
            raft.transform.position = spawnPoint.position;
            raft.GetComponent<Raft>().Initialize();

            QueuedRaft queuedRaft;
            queuedRaft.raft = raft.GetComponent<Raft>();
            queuedRaft.queueLocation = FindEmptyQueueSlot();
            queuedRaft.ditchQueueTime = Time.time + Random.Range(waitInQueueDurationMinMax.x, waitInQueueDurationMinMax.y);

            Vector3 destinationPosition = GetQueueRaftPosition(queuedRaft);
            queuedRaft.raft.transform.DOMove(destinationPosition, moveIntoQueueDuration).SetEase(moveIntoQueueEase);

            queuedRafts.Add(queuedRaft);
        }
    }

    private GameObject PickRaftType()
    {
        return raftTypes[GetWeightedRaftIndex()];
    }

    private void DitchBoat(int index) 
    {
        Raft raft = queuedRafts[index].raft;

        raft.transform.DOMove(spawnPoint.position, ditchMoveDuration).SetEase(ditchEase);
        Destroy(raft.gameObject, ditchMoveDuration + 1f);

        RemoveRaftFromQueue(index);
    }

    private void RemoveRaftFromQueue(int index)
    {
        queuedRafts.RemoveAt(index);
    }

    private int FindEmptyQueueSlot()
    {   
        for (int i = 0; i < queueSize; i++)
        {
            //if no raft exists in this list then fill that slot
            if (queuedRafts.Find(r => r.queueLocation == i).raft == null)
            {
                return i;
            }
        }

        Debug.LogError("Uh oh we returned -1");
        return -1;
        
    }

    private void UpdateSelection()
    {
        float stepSize = queueWidth / (queueSize - 1);
        raftSelector.transform.position = transform.position + (queueDirection * stepSize * currentSelection) - queueDirection * queueWidth * 0.5f + Vector3.up * SELECTORHEIGHT;
    }

    private Vector3 GetQueueRaftPosition(QueuedRaft queuedRaft)
    {
        float stepSize = queueWidth / (queueSize - 1);

        return transform.position + (queueDirection * stepSize * queuedRaft.queueLocation) - queueDirection * queueWidth * 0.5f;
    }

    private int GetWeightedRaftIndex()
    {
        int weightIndex = Random.Range(0, weightedIndexArray.Length);

        return weightedIndexArray[weightIndex];
    }
}

public struct QueuedRaft
{
    public Raft raft;
    public int queueLocation;
    public float ditchQueueTime;
}
