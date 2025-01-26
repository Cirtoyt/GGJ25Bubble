using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI : MonoSingleton<UI>
{

    // Health Variables
    public int Health;
    [SerializeField] int NumHearts;
    [SerializeField] Image[] Hearts;
    [SerializeField] Sprite FullHeart;
    [SerializeField] Sprite EmptyHeart;
    [SerializeField] int MaxHearts;

    // Timer Variables
    [SerializeField] float timeRemaing = 40.0f;
    [SerializeField] TextMeshProUGUI timerText;

    // Progress Bar variables

    public int current;
    public int max;
    public Image mask;

    // powerup counter variables
    [SerializeField] int[] powerUpAmmo;
    [SerializeField] TextMeshProUGUI[] powerupAmmoText;

    // Damage? 

    protected override void Awake()
    {
        base.Awake();

        MaxHearts = 6;
    }

    void Update()
    {
        SetHealth();
        SetTimer();
        GetFill();
        Damage();
        addPowerUp();
    }

    void SetHealth()
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
        if (timeRemaing < 30)
        {
            timerText.color = Color.red;
        }
        int mins = Mathf.FloorToInt(timeRemaing / 60);
        int secs = Mathf.FloorToInt(timeRemaing % 60);
        timerText.text = timeRemaing.ToString();
        timerText.text = string.Format("{0:00} : {1:00}", mins, secs);
    }

    void GetFill()
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
    }

    void addPowerUp()
    {
        for (int i = 0; i < 2; i++)
        {
            //powerupAmmoText[i].text = powerUpAmmo[i].ToString();
        }
        addKunai();
        addShuriken();

    }

    void addKunai()
    {
        // this will overlap events
        if (Input.GetKeyDown(KeyCode.X))
        {
            powerUpAmmo[0]++;
        }
        // when powerup used
        if (Input.GetKeyDown(KeyCode.Y))
        {
            powerUpAmmo[0]--;
        }
    }

    void addShuriken()
    {
        // this will overlap events
        if (Input.GetKeyDown(KeyCode.Q))
        {
            powerUpAmmo[1]++;
        }
        // when powerup used
        if (Input.GetKeyDown(KeyCode.W))
        {
            powerUpAmmo[1]--;
        }
    }
}
