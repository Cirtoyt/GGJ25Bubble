using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Spaceship : MonoSingleton<Spaceship>
{
    [SerializeField] private bool followLevelSpline = true;

    private void Start()
    {
        if (followLevelSpline)
        {
            transform.position = GameManager.Instance.LevelSpline.EvaluatePosition(0);
        }
    }

    private void Update()
    {
        if (followLevelSpline)
        {
            transform.position = GameManager.Instance.LevelSpline.EvaluatePosition(UI.Instance.GetTimerProgress());
        }
    }
}
