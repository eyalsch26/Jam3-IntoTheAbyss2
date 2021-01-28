using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        power = maxPower;
        StartCoroutine(chargePower(powerChargePerSec));
    }

    void Update()
    {
        //HUD update
        int discretePower = (int) Mathf.Floor(power);
        for (int i = 0; i < discretePower; i++)
        {
            powerBar[i].sprite = elements[2]; // on parts
        }
        for (int i = discretePower; i < maxPower; i++)
        {
            powerBar[i].sprite = elements[3]; // off parts
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
