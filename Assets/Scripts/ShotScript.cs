using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour
{
    
    public bool playerShot;
    public float playerShotSpeed;
    public float enemyShotSpeed;
    private float currShotSpeed;
    public GameObject pShot;
    public GameObject eShot;
    public float lifeTime;
    private float currLifeTime;
    public static HashSet<string> nonBlockingObjectTags = new HashSet<string> { 
    "Laser", "Rope", "Shot", "EnemyShot", "Iodine", "Health"};

    // Start is called before the first frame update
    void Start()
    {
        currLifeTime = lifeTime;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float sloMoFactor = (Time.timeScale < 1 && playerShot) ? 3 : 1;
        Vector3 move = transform.up * Time.deltaTime * currShotSpeed * sloMoFactor;
        transform.position += move;
        currLifeTime -= Time.deltaTime;
        Debug.Log(currLifeTime);
        if (currLifeTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void setUpShot(bool isPlayer)
    {
        playerShot = isPlayer;
        pShot.SetActive(isPlayer);
        eShot.SetActive(!isPlayer);
        currLifeTime = lifeTime;
        currShotSpeed = (isPlayer) ? playerShotSpeed : enemyShotSpeed;
        gameObject.tag = (isPlayer) ? "Shot" : "EnemyShot";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (nonBlockingObjectTags.Contains(collision.tag))
        {
            return;
        }
        else if ((collision.tag == "Ghost" && !playerShot) || 
            (collision.tag == "Player" && playerShot))
        {
            return;
        }
            gameObject.SetActive(false);
    }

}