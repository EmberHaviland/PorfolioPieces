using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class LineView : MonoBehaviour
{
    public LineRenderer lineRend;
    public LineRenderer leadRend;

    public PlayerBallLogic pb;

    Color startCol1;
    Color startCol2;

    private void Awake()
    {
        startCol1 = lineRend.colorGradient.colorKeys[0].color;
        startCol2 = lineRend.colorGradient.colorKeys[1].color;
    }

    public void RenderLine(Vector3 _startPoint, Vector3 _endPoint)
    {
        lineRend.positionCount = 2;
        Vector3[] points = new Vector3[2];
        points[0] = _startPoint;
        points[1] = _endPoint;

        leadRend.positionCount = 2;
        Vector3[] pointsLead = new Vector3[2];
        pointsLead[0] = _startPoint;
        pointsLead[1] = (((_startPoint - _endPoint) * 0.75f) + (_startPoint - _endPoint).normalized * 0.4f) + _startPoint;
        leadRend.SetPositions(pointsLead);


        float ColorShiftPercentage = new Vector3(_startPoint.x - _endPoint.x, _startPoint.y - _endPoint.y, 0).magnitude / new Vector3(pb.maxPow.x, pb.maxPow.y, 0).magnitude;

        Color newColStrong = new Color(startCol1.r + ((108.0f / 255.0f) * ColorShiftPercentage), startCol1.g, startCol1.b - ((108.0f / 255.0f) * ColorShiftPercentage));
        Color newColWeak = new Color(startCol2.r + ((108.0f / 255.0f) * ColorShiftPercentage), startCol2.g - ((81.0f / 255.0f) * ColorShiftPercentage), startCol2.b - ((108.0f / 255.0f) * ColorShiftPercentage));
        newColWeak.a = 0;
        lineRend.startColor = newColStrong;
        lineRend.endColor = newColWeak;

        lineRend.SetPositions(points);
    }

    public void EndLine()
    {
        lineRend.positionCount = 0;
        leadRend.positionCount = 0;
        // Reset colors.
        lineRend.startColor = startCol1;
        lineRend.endColor = startCol2;
    }
}
