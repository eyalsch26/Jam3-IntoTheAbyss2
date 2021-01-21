using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public int health;
    public float speed;
    public GameObject ghost;
    GameController manager;
    public float shootDistance;
    public float avgShotTimeGap;
    Transform pTransform;
    bool alive;

    // Start is called before the first frame update
    void Start()
    {
        alive = true;
        StartCoroutine(shootAtPlayerCycle());
        manager = GameObject.Find("Level").GetComponent<GameController>();
        pTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((pTransform.position - ghost.transform.position).magnitude < 10)
        {
            followPlayer();
        }
    }

    public void turnLeft(bool isLeft)
    {
        if (isLeft)
        {
            ghost.transform.localRotation = Quaternion.Euler(0, 225, 0);
        }

        else ghost.transform.localRotation = Quaternion.Euler(0, 135, 0);
    }


    void followPlayer()
    {
        Vector3 direction = (pTransform.position - ghost.transform.position).normalized * Time.deltaTime * speed;
        transform.position += direction;
        turnLeft(direction.x < 0);
    }

    IEnumerator shootAtPlayerCycle()
    {
        GameObject shot;
        while (alive)        
        {
            Vector3 shootDirection = pTransform.position - ghost.transform.position;
            if ((ghost.transform.position - pTransform.position).magnitude < shootDistance)
            {
                shot = manager.getShot(false);
                shot.transform.position = transform.position + 0.1f * shootDirection.normalized;
                shot.transform.up = shootDirection;
            }
            float t = Random.Range(-1, 1);
            yield return new WaitForSeconds(avgShotTimeGap + t);
        }
    }

    void takeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            alive = false;
            gameObject.SetActive(false);
        }    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("enemy hit!");
        if (collision.tag == "Shot")
        {
            takeHit(1);
        }
    }
}
