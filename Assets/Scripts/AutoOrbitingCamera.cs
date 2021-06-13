using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoOrbitingCamera : MonoBehaviour
{
    public float orbitSpeed = 10f;
    public Transform target;


    void Update()
    {
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
    }
}
