using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//quadratic
public class BezierCurve
{
    public Vector3 startPoint; //P0
    public Vector3 endPoint; //P2
    public Vector3 anchorPoint; //P1

    public BezierCurve(Vector3 _startPoint, Vector3 _endPoint, Vector3 _anchorPoint)
    {
        startPoint = _startPoint;
        endPoint = _endPoint;
        anchorPoint = _anchorPoint;
    }

    public Vector3 Evaluate(float t)
    {
        return anchorPoint + Mathf.Pow((1f - t), 2f) * (startPoint - anchorPoint) + Mathf.Pow(t, 2f) * (endPoint - anchorPoint);
    }
}
