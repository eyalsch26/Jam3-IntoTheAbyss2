using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour
{
    GameController manager;
    public int iodineReward;
    public GameObject barrel;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Level").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator explode()
    {
        barrel.SetActive(false);
        explosion.SetActive(true);
        GetComponent<Collider2D>().enabled = false;
        manager.PositionPills(barrel.transform.position.x, barrel.transform.position.y, 5);
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shot")
        {
            StartCoroutine(explode());
        }
    }

}
