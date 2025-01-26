using NUnit;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.UI.Image;

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
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
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
                                    Spaceship.Instance.transform.position,
                                    PlayerController.Instance.transform.position);
    }

    private float GetProgressAlongLine(Vector3 lineStart, Vector3 lineEnd, Vector3 pointAlongLine)
    {
        Vector3 line = lineEnd - lineStart;

        // Check layer is behind line start
        if (Vector3.Dot(pointAlongLine - lineStart, line) < 0)
            return 0;

        Vector3 distanceAlongLine = Vector3.Project(pointAlongLine - lineStart, line);
        //Vector3 nearestWorldPointAlongLine = lineStart + distanceAlongLine;

        // Clamp to values over 1 and set to 1
        return Mathf.Clamp01(distanceAlongLine.magnitude / line.magnitude);
    }

    public void FailGame()
    {
        Debug.Log("Game Lost!");
    }

    public void WinGame()
    {
        Debug.Log("Game Win!");
    }
}
