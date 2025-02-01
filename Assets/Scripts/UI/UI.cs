using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI : MonoSingleton<UI>
{
    [Header("Off-screen Indicator Variables")]
    [SerializeField] Canvas _parentCanvas;
    [Range(0f, 1f)]
    [SerializeField] private float _indicatorBoundsMultiplier = 0.9f;
    //[SerializeField] private float _indicatorBoundsOffset = 10f;
    [SerializeField] private RectTransform _spaceshipIndicatorImage;
    [SerializeField] private RectTransform _spaceshipIndicatorIconImage;

    [Header("Health Variables")]
    public int Health;
    [SerializeField] int NumHearts;
    [SerializeField] Image[] Hearts;
    [SerializeField] Sprite FullHeart;
    [SerializeField] Sprite EmptyHeart;
    [SerializeField] int MaxHearts;
    [SerializeField] private AudioSource _healAudioSource;

    [Header("Timer Variables")]
    [SerializeField] float timeRemaing = 40.0f;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] private float endChaseTime = 30;

    [Header("Progress Bar Variables")]
    public int current;
    public int max;
    public Image mask;

    [Header("Powerup Counter Variables")]
    [SerializeField] List<int> powerupAmmo = new List<int>();
    [SerializeField] List<Image> powerupIcons = new List<Image>();
    [SerializeField] List<TextMeshProUGUI> powerupAmmoText = new List<TextMeshProUGUI>();
    [SerializeField] private AudioSource _pickUpPowerUpAudioSource;

    public float TimeRemaining => timeRemaing;
    public float EndChaseTime => endChaseTime;

    protected override void Awake()
    {
        base.Awake();

        MaxHearts = 6;

        foreach (Image icon in powerupIcons)
        {
            icon.color = Color.grey;
        }
    }

    void Update()
    {
        SetTargetPointersUI();

        SetTimer();
        SetTimerUIFill();

        SetHealthUI();

        UpdatePowerupUI();
    }

    private void SetTargetPointersUI()
    {
        // Collect positions
        Vector3 spaceshipScreenPosition = PlayerController.Instance.Cam.WorldToScreenPoint(Spaceship.Instance.transform.position);
        Debug.Log($"spaceshipScreenPosition: {spaceshipScreenPosition}");

        //EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
        //List<Vector3> enemyScreenPositions = new List<Vector3>();
        //foreach(EnemyController enemy in enemies)
        //{
        //    enemyScreenPositions.Add(PlayerController.Instance.Cam.WorldToScreenPoint(enemy.transform.position));
        //}

        // Detect on/off screen
        if (CheckObjectIsOnscreen(spaceshipScreenPosition))
        {
            // On-screen, hide indicator
            _spaceshipIndicatorImage.gameObject.SetActive(false);
        }
        else
        {
            // Off-screen, show pointing indicator
            _spaceshipIndicatorImage.gameObject.SetActive(true);

            CalculateOffScreenIndicatorPosition(spaceshipScreenPosition, out Vector3 indicatorScreenPosition, out float indicatorAngle);

            _spaceshipIndicatorImage.localPosition = indicatorScreenPosition;
            _spaceshipIndicatorImage.localRotation = Quaternion.Euler(0, 0, indicatorAngle);
            _spaceshipIndicatorIconImage.localRotation = Quaternion.Euler(0, 0, -indicatorAngle);
        }
    }

    private bool CheckObjectIsOnscreen(Vector3 objectScreenPosition)
    {
        return objectScreenPosition.z > 0
            && objectScreenPosition.x > 0 && objectScreenPosition.x <= Screen.width
            && objectScreenPosition.y > 0 && objectScreenPosition.y <= Screen.height;
    }

    private void CalculateOffScreenIndicatorPosition(Vector3 objectScreenPosition, out Vector3 indicatorScreenPosition, out float indicatorDegAngle)
    {
        // Check to mirror screen position when spaceship is behind the camera
        if (objectScreenPosition.z < 0)
        {
            //objectScreenPosition = -objectScreenPosition;
            objectScreenPosition *= -1;
            //objectScreenPosition.x = Screen.width - objectScreenPosition.x;
            //objectScreenPosition.y = Screen.height - objectScreenPosition.y;
        }

        Vector3 screenCentre = new Vector3(Screen.width, Screen.height) * 0.5f;

        // Temp shift position to 0,0 so can be used with atan2
        objectScreenPosition -= screenCentre;

        float angle = Mathf.Atan2(objectScreenPosition.y, objectScreenPosition.x);
        angle -= 90 * Mathf.Deg2Rad;

        // Return the angle
        indicatorDegAngle = angle * Mathf.Rad2Deg;

        // Get gradiant
        float cos = Mathf.Cos(angle);
        float sin = -Mathf.Sin(angle);

        // y = mx + b format
        float m = cos / sin;

        // Lock image to bounds based on angle from 0, 0 (the shifted centre of screen position)
        Vector3 screenBounds = (screenCentre * _indicatorBoundsMultiplier) / _parentCanvas.scaleFactor;
        Debug.Log($"screenBounds: {screenBounds}");

        // First check up & down
        // x = y/m
        if (cos > 0) // Check up
            objectScreenPosition = new Vector3(screenBounds.y / m, screenBounds.y, 0);
        else // Check down
            objectScreenPosition = new Vector3(-screenBounds.y / m, -screenBounds.y, 0);

        // If out of bounds along the x axis, get point on appropriate side of the screen
        // y = m * x
        if (objectScreenPosition.x > screenBounds.x) // Check right
            objectScreenPosition = new Vector3(screenBounds.x, screenBounds.x * m);
        else if (objectScreenPosition.x < -screenBounds.x) // Check left
            objectScreenPosition = new Vector3(-screenBounds.x, -screenBounds.x * m);

        // Undo the position shift to 0,0, and return position to screen centre
        //objectScreenPosition += screenCentre;

        // Set final transformed indicator position
        indicatorScreenPosition = objectScreenPosition;
    }

    private void SetTimer()
    {
        if (timeRemaing < 0)
        {
            timeRemaing = 0;
        }
        if (timeRemaing > max)
        {
            timeRemaing = max;
        }
        if (timeRemaing > 0)
        {
            timeRemaing -= Time.deltaTime;
        }
        if (timeRemaing < endChaseTime)
        {
            timerText.color = Color.red;
        }
        int mins = Mathf.FloorToInt(timeRemaing / 60);
        int secs = Mathf.FloorToInt(timeRemaing % 60);
        timerText.text = timeRemaing.ToString();
        timerText.text = string.Format("{0:00} : {1:00}", mins, secs);
    }

    private void SetTimerUIFill()
    {
        current = (int)timeRemaing;

        float fillAmount = (float)current / (float)max;
        mask.fillAmount = fillAmount;
    }

    public float GetTimerProgress(bool reverse = false)
    {
        float timerValueProgressAgainstMax = (float)timeRemaing / (float)max;

        if (!reverse)
            timerValueProgressAgainstMax = 1 - timerValueProgressAgainstMax;

        return timerValueProgressAgainstMax;
    }

    void SetHealthUI()
    {
        for (int i = 0; i < Hearts.Length; i++)
        {
            if (i < NumHearts)
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }

            if (i < Health)
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }
        }
    }

    public void Damage()
    {
        if (Health > 0)
            Health--;

        if (Health <= 0)
            PlayerController.Instance.OnDeath();
    }

    public void Heal()
    {
        if (Health < MaxHearts)
            Health++;

        _healAudioSource.Play();
    }

    private void UpdatePowerupUI()
    {
        for (int i = 0; i < powerupAmmoText.Count; i++)
        {
            powerupAmmoText[i].text = powerupAmmo[i].ToString();
        }
    }

    public int GetAmmoForPowerUp(Weapon.WeaponType weaponType)
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.Shuriken:
                return powerupAmmo[0];
            case Weapon.WeaponType.Harpoon:
                return powerupAmmo[1];
            default:
                return 0;
        }
    }

    public void AddPowerUp(Weapon.WeaponType weaponType, int ammoAmount)
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.Shuriken:
                AddShuriken(ammoAmount);
                break;
            case Weapon.WeaponType.Harpoon:
                AddHarpoon(ammoAmount);
                break;
        }

        _pickUpPowerUpAudioSource.Play();
    }

    public void UseWeapon(Weapon.WeaponType weaponType)
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.Shuriken:
                RemoveShuriken();
                break;
            case Weapon.WeaponType.Harpoon:
                RemoveHarpoon();
                break;
        }
    }

    private void AddShuriken(int addedAmmo)
    {
        powerupIcons[0].color = Color.white;

        powerupAmmo[0] += addedAmmo;
    }

    private void RemoveShuriken()
    {
        powerupAmmo[0] -= 1;

        if (powerupAmmo[0] <= 0)
        {
            powerupIcons[0].color = Color.grey;
        }
    }

    private void AddHarpoon(int addedAmmo)
    {
        powerupIcons[1].color = Color.white;

        powerupAmmo[1] += addedAmmo;
    }

    private void RemoveHarpoon()
    {
        powerupAmmo[1] -= 1;

        if (powerupAmmo[1] <= 0)
        {
            powerupIcons[1].color = Color.grey;
        }
    }
}
