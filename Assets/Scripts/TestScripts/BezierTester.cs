using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierTester : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform anchorPoint;

    public int accuracy = 10;
    // Start is called before the first frame update
    void Start()
    {
        BezierCurve curve = new BezierCurve(startPoint.position, endPoint.position, anchorPoint.position);

        for (int i = 0; i < accuracy; i++)
        {
            float t = (float)i / (float)(accuracy - 1);

            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = curve.Evaluate(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
