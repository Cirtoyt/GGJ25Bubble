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
        Vector3 startPos = _levelSplineContainer.EvaluatePosition(0);
        Vector3 endPos = _levelSplineContainer.EvaluatePosition(1);

        var wander = PlayerController.Instance.transform.position - startPos;
        Vector3 span = endPos - startPos;

        // Compute how far along the line is the closest approach to our point.
        float t = Vector3.Dot(wander, span) / span.sqrMagnitude;

        // Restrict this point to within the line segment from start to end.
        t = Mathf.Clamp01(t);

        Vector3 nearest = startPos + t * span;
        float distanceAlongLevelVector = (nearest - PlayerController.Instance.transform.position).magnitude;
        
        // Get 0-1 progress value
        return distanceAlongLevelVector / span.magnitude;
    }
}
