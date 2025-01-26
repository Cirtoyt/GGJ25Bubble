using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private SplineContainer _levelSplineContainer;

    public SplineContainer LevelSpline => _levelSplineContainer;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        
    }

    public float GetPlayerProgressAlongLevelSpline()
    {
        return GetProgressAlongLine(_levelSplineContainer.EvaluatePosition(0),
                                    _levelSplineContainer.EvaluatePosition(1),
                                    PlayerController.Instance.transform.position);
    }

    public float GetPlayerProgressTowardsSpaceship()
    {
        return GetProgressAlongLine(_levelSplineContainer.EvaluatePosition(0),
                                    PlayerController.Instance.transform.position,
                                    Spaceship.Instance.transform.position);
    }

    private float GetProgressAlongLine(Vector3 lineStart, Vector3 lineEnd, Vector3 pointAlongLine)
    {
        var wander = pointAlongLine - lineStart;
        Vector3 span = lineEnd - lineStart;

        // Compute how far along the line is the closest approach to our point.
        float t = Vector3.Dot(wander, span) / span.sqrMagnitude;

        // Restrict this point to within the line segment from start to end.
        t = Mathf.Clamp01(t);

        Vector3 nearest = lineStart + t * span;
        float distanceAlongLevelVector = (nearest - pointAlongLine).magnitude;

        // Get 0-1 progress value
        return distanceAlongLevelVector / span.magnitude;
    }
}
