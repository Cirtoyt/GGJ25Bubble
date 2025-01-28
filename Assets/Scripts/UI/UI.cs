using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI : MonoSingleton<UI>
{
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
        SetTimer();
        SetTimerUIFill();

        SetHealthUI();

        UpdatePowerupUI();
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

    void SetTimer()
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

    void SetTimerUIFill()
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
