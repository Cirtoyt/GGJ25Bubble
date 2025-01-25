using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Spaceship : MonoSingleton<Spaceship>
{
    [SerializeField] private SplineContainer spline;

    private void Start()
    {
        transform.position = spline.EvaluatePosition(0);
    }

    private void Update()
    {
        transform.position = spline.EvaluatePosition(UI.Instance.GetTimerProgress());
        Debug.Log(UI.Instance.GetTimerProgress());
    }
}
