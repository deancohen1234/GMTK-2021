using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftTester : MonoBehaviour
{
    public Raft raft1;
    public Raft raft2;

    public ConnectionDirection setDirection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            raft2.JoinRaft(raft1, setDirection);
        }   
    }
}
