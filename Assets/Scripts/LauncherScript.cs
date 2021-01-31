using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherScript : MonoBehaviour
{
    public Transform xPartTransform;
    public Transform yPartTransform;
    public Transform sensorTransform;
    public GameObject launcher;
    
    public GameObject lockEye;
    public GameObject explosion;
    Transform playerTransform;
    GameController manager;

    public GameObject missile;
    public MissileScript missileScript;
    public Transform missilePlace;

    bool isActive = false;
    bool isLockedOnPlayer = false;
    public float activeDistance;
    public float lockDistance;
    public float rotationSpeed;

    public int health;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Level").GetComponent<GameController>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        StartCoroutine(searching());
    }

    // Update is called once per frame
    void Update()
    {
        // inactive -> searching
        if (!isActive && (transform.position - playerTransform.position).magnitude <= activeDistance)
        {
            isActive = true;
            StartCoroutine(searching());
        }
        // searching -> locked on player
        else if (isActive && (transform.position - playerTransform.position).magnitude <= lockDistance)
        {
            lockEye.SetActive(true);
            isLockedOnPlayer = true;
            xPartTransform.forward = playerTransform.position - transform.position;
            yPartTransform.eulerAngles = new Vector3(0, 100, 0);
            if (!missileScript.isActive)
            {
                StartCoroutine(launchMissile());
            }
        }
        // locked -> searching
        else if (isLockedOnPlayer && (transform.position - playerTransform.position).magnitude > lockDistance)
        {
            lockEye.SetActive(false);
            isLockedOnPlayer = false;
            StartCoroutine(searching());
        }
        // searching -> inactive
        else if (isActive && (transform.position - playerTransform.position).magnitude > activeDistance)
        {
            isActive = false;

        }
    }

    IEnumerator searching()
    {
        int xDir = 1;
        int yDir = 1;
        while (isActive && !isLockedOnPlayer)
        {
            yPartTransform.Rotate(0, yDir * Time.deltaTime * rotationSpeed, 0);
            if (yPartTransform.eulerAngles.y > 111)
            {
                yDir = -1; 
            }
            else if (yPartTransform.eulerAngles.y < 81) 
            {
                yDir = 1; 
            }
            xPartTransform.Rotate(xDir * Time.deltaTime * rotationSpeed, 0, 0);
            Debug.Log(xPartTransform.rotation.x);
            if (xPartTransform.rotation.x > 0.19)
            {
                xDir = -1;
            }
            else if (xPartTransform.rotation.x < - 0.13)
            {
                xDir = 1;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator launchMissile()
    {
        yield return new WaitForSeconds(0.5f);
        if (launcher.activeSelf && isLockedOnPlayer)
        {
            missileScript.getLaunched(playerTransform);
        }
    }

    public void resetMissile()
    {
        missile.transform.position = missilePlace.position;
        missile.transform.rotation = Quaternion.Euler(new Vector3(0, -87, 0));
        missileScript.resetMissile();
    }

    void takeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            //alive = false;
            StartCoroutine(LauncherKilled());
        }
    }

    IEnumerator LauncherKilled()
    {
        launcher.SetActive(false);
        explosion.SetActive(true);
        GetComponent<Collider2D>().enabled = false;
        manager.PositionPills(transform.position.x, transform.position.y, 3);
        yield return new WaitForSeconds(1.5f);
        yield return new WaitWhile(missileScript.getActive);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shot")
        {
            takeHit(1);
        }
    }
}
