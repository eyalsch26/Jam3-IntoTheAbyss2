using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public playerMovement controller;
    public GameController manager;

    public int maxHealth;
    public float maxPower;
    public int iodine;
    public float powerChargePerSec;
    public float ropePowerUsage;
    public float shotPowerUsage;
    public float shieldPowerUsagePerSec;
    private int health;
    private float power;

    // UI elements:
    public List<Image> hearts;
    public List<Image> powerBar;
    public TextMeshProUGUI iodineAmount;
    public Image IodineIcon;
    public Image ropeIcon;
    public Image gunIcon;
    public Image shieldIcon;
    public Image countDownHUAnimation;
    public Image modeWheel;
    public List<Sprite> elements;
    //0- heartOn;
    //1-heartOff;
    //2- barOn;
    //3- barOff;
    //4- ropeOn;
    //5- ropeOff;
    //6- gunOn;
    //7- gunOff;
    //8- shieldOn;
    //9- shieldOff;
    //10- IodineOn;
    //11- IodineOff;
    //12- wheel rope
    //13- wheel gun
    //14- wheel shield


    // mouseWheel vars:
    public bool isModeChoosing;
    Vector2 wheelCenter;
    Vector2 mousePos;
    char currMode = 'r';


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        power = maxPower;
        StartCoroutine(chargePower(powerChargePerSec));
        StartCoroutine(iodineCountDown(1f));
        StartCoroutine(countDownHeadsUp());
    }

    void Update()
    {
        //HUD update
        if (isModeChoosing)
        {
            MaintainModeWheel();
        }
        int discretePower = (int) Mathf.Floor(power);
        for (int i = 0; i < discretePower; i++)
        {
            powerBar[i].sprite = elements[2]; // on parts
        }
        for (int i = discretePower; i < maxPower; i++)
        {
            powerBar[i].sprite = elements[3]; // off parts
        }
        if (iodine <= 0)
        {
            manager.gameOver();
        }
    }

        public void healthDown()
    {
        health--;
        if (health == 0)
        {
            manager.gameOver();
        }
        else
        {
            hearts[health].sprite = elements[1];
        }
    }

    public void healthUp()
    {
        health++;
    }

    public bool tryUsePower(char ability)
    {
        float price;
        if (ability == 'r')
        {
            price = ropePowerUsage;
        }
        else if (ability == 's')
        {
            price = shieldPowerUsagePerSec;
        }    
        else
        {
            price = shotPowerUsage;
        }
        if  (price <= power)
        {
            power -= price;
            return true;
        }
        return false;
    }

    IEnumerator chargePower(float chargeRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(chargeRate);
            if (power < maxPower && !controller.shieldOn)
            {
                power = Mathf.Min(maxPower, power + 1);
            }    
        }    
    }

    IEnumerator iodineCountDown(float dropRate)
    {
        yield return new WaitForSeconds(2f);
        while (iodine > 0)
        {
            yield return new WaitForSecondsRealtime(dropRate);
            setIodine(-1);
        }
    }

    IEnumerator countDownHeadsUp()
    {
        yield return new WaitForSeconds(2f);
        countDownHUAnimation.enabled = true;
        for (int i = 0; i < 3; i++)
        {
            float t = 0;
            float width;
            while (t < 0.9)
            {
                width = Mathf.Floor(t * t * (10f / 9f) * 91);
                countDownHUAnimation.rectTransform.sizeDelta = new Vector2(width, 3);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
                

            }
            yield return new WaitForSeconds(0.1f);
        }
        countDownHUAnimation.enabled = false;
    }

    public void setMode(char mode)
    {
        switch (mode)
        {
            case 'r':
                ropeIcon.enabled = true;
                gunIcon.enabled = false;
                shieldIcon.enabled = false;
                break;
            case 'f':
                ropeIcon.enabled = false;
                gunIcon.enabled = true;
                shieldIcon.enabled = false;
                break;
            case 's':
                ropeIcon.enabled = false;
                gunIcon.enabled = false;
                shieldIcon.enabled = true;
                break;
        }
    }

    public void startModeWheel()
    {
        modeWheel.enabled = true;
        Vector2 pos = Mouse.current.position.ReadValue();
        modeWheel.rectTransform.position = pos;
        wheelCenter = pos;
        isModeChoosing = true;
    }

    void MaintainModeWheel()
    {
        mousePos = Mouse.current.position.ReadValue();
        //Debug.Log((mousePos - wheelCenter).magnitude);
        if ((mousePos - wheelCenter).magnitude > 26 && (mousePos - wheelCenter).magnitude < 126)
        {
            float rad = Mathf.Atan2((mousePos - wheelCenter).y, (mousePos - wheelCenter).x);
            if (rad <= -Mathf.PI / 6 && rad > -5 * Mathf.PI / 6) // gun
            {
                modeWheel.sprite = elements[13];
                controller.changeMode('f');
            }
            else if (rad <= -5 * Mathf.PI / 6 || rad > Mathf.PI / 2) // shield
            {
                modeWheel.sprite = elements[14]; 
                controller.changeMode('s');
            }
            else // rope
            {
                modeWheel.sprite = elements[12];
                controller.changeMode('r');
            }
        }
    }

    public void closeModeWheel()
    {
        modeWheel.enabled = false;
        isModeChoosing = false;
    }

    public bool setIodine(int amount)
    {
        if (iodine + amount < 0)
        { return false; }
        iodine += amount;
        iodineAmount.text = iodine.ToString();
        return true;
    }

    public void IodineOn()
    {
        IodineIcon.sprite = elements[10];
    }
    public void IodineOff()
    {
        IodineIcon.sprite = elements[11];
    }

}
