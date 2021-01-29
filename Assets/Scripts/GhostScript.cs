using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    public int health;
    public float speed;
    public GameObject ghost;
    public GameObject puff;
    GameController manager;
    public float shootDistance;
    public float avgShotTimeGap;
    Transform pTransform;
    public Transform gunTransform;
    bool alive;

    // Start is called before the first frame update
    void Start()
    {
        alive = true;
        manager = GameObject.Find("Level").GetComponent<GameController>();
        pTransform = GameObject.Find("Player").GetComponent<Transform>();
        StartCoroutine(shootAtPlayerCycle());
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
        //MG aim
        gunTransform.forward = direction;
    }

    IEnumerator shootAtPlayerCycle()
    {
        GameObject shot;
        while (alive)        
        {
            float t = Random.Range(-1, 1);
            yield return new WaitForSeconds(avgShotTimeGap + t);
            Vector3 shootDirection = pTransform.position - ghost.transform.position;
            if (alive && (ghost.transform.position - pTransform.position).magnitude < shootDistance)
            {
                shot = manager.getShot(false);
                shot.transform.position = transform.position + 0.2f * shootDirection.normalized;
                shot.transform.up = shootDirection;
            }
        }
    }

    void takeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            alive = false;
            StartCoroutine(ghostKilled());
        }    
    }

    IEnumerator ghostKilled()
    {
        ghost.SetActive(false);
        puff.SetActive(true);
        GetComponent<Collider2D>().enabled = false;
        manager.PositionPills(ghost.transform.position.x, ghost.transform.position.y, 3);
        yield return new WaitForSeconds(1.5f);
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
