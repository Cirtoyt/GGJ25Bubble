using NUnit;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;
using static UnityEngine.UI.Image;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("Statics")]
    [SerializeField] private SplineContainer _levelSplineContainer;
    [SerializeField] private AudioSource _mainChaseMusic;
    [SerializeField] private AudioSource _endChaseMusic;
    [Header("Properties")]
    [SerializeField] private float _endChaseMusicFadeDuration = 5;

    public SplineContainer LevelSpline => _levelSplineContainer;

    protected override void Awake()
    {
        base.Awake();

        _mainChaseMusic.volume = 1;
        _endChaseMusic.volume = 0;
        _endChaseMusic.mute = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        UpdateMusicVolumes();
    }

    private void UpdateMusicVolumes()
    {
        if (UI.Instance.TimeRemaining <= UI.Instance.EndChaseTime - _endChaseMusicFadeDuration)
        {
            _mainChaseMusic.mute = true;
            _endChaseMusic.volume = 1;
        }
        else if (UI.Instance.TimeRemaining <= UI.Instance.EndChaseTime)
        {
            _endChaseMusic.mute = false;

            float fadeStart = UI.Instance.EndChaseTime;
            float fadeEnd = fadeStart - _endChaseMusicFadeDuration;
            float currentFade = (_endChaseMusicFadeDuration - (UI.Instance.TimeRemaining - fadeStart)) / _endChaseMusicFadeDuration;
            // Fix:
            currentFade -= 1;

            _mainChaseMusic.volume = 1 - currentFade;
            _endChaseMusic.volume = currentFade + 0.1f;
        }
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
        StartCoroutine(LoseGameSequence());
    }

    private IEnumerator LoseGameSequence()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(3);
    }

    public void WinGame()
    {
        Debug.Log("Game Win!");
        StartCoroutine(WinGameSequence());
    }

    private IEnumerator WinGameSequence()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        EnemyController[] allEnemies = FindObjectsByType<EnemyController>(sortMode: FindObjectsSortMode.None);
        foreach(EnemyController enemy in allEnemies)
        {
            Destroy(enemy.gameObject);
        }

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(2);
    }
}
