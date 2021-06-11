using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//North = Z Forward (Global) +
//South = Z Backward (Global) -
//East = X Forward (Global) +
//West = X Backward (Global) -
public class Raft : MonoBehaviour
{
    public Collider northCollider;
    public Collider southCollider;
    public Collider eastCollider;
    public Collider westCollider;

    void Start()
    {
        if (!northCollider || !southCollider || !eastCollider || westCollider)
        {
            Debug.LogError("No Collider Set For Raft damnit");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
