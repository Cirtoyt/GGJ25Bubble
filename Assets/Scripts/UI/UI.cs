using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
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



    void Start()
    {
        MaxHearts = 6;
    }

    // Update is called once per frame
    void Update()
    {
        SetHealth();
        SetTimer();
        GetFill();
        Damage();
    }

    void SetHealth()
    {
        for (int i = 0; i < Hearts.Length; i++)
        {
            if( i < NumHearts )
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }

            if ( i < Health )
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
        if(timeRemaing < 0)
        {
            timeRemaing = 0;
        }
        if (timeRemaing > 0)
        {
            timeRemaing -= Time.deltaTime;
        }
        if(timeRemaing < 30)
        {
            timerText.color = Color.red;
        }
        int mins = Mathf.FloorToInt( timeRemaing / 60 );
        int secs = Mathf.FloorToInt(timeRemaing % 60 );
        timerText.text = timeRemaing.ToString();    
        timerText.text = string.Format("{0:00} : {1:00}", mins, secs);
    }

    void GetFill()
    {
        current = (int)timeRemaing;
        
        float fillAmount = (float)current / (float)max;
        mask.fillAmount = fillAmount;
    }

    void Damage()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Health--;
        }
        
        if(Health < MaxHearts)
        if (Input.GetMouseButtonDown(1))
        {
            Health++;
        }
    }

}
