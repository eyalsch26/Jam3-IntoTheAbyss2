using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
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
        iodineAmount.text = discretePower.ToString();
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
            if (power < maxPower)
            {
                power = Mathf.Min(maxPower, power + 1);
            }    
        }    
    }

    public void sloMoStart()
    {

    }

    public void sloMoEnd()
    {

    }
}
