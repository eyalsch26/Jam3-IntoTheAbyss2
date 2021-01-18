using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    GameController manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Level").GetComponent<GameController>();
        StartCoroutine(startRadiationFire());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator startRadiationFire()
    {
        while(gameObject.activeSelf)
        {
            yield return new WaitForSeconds(3f);
            int angle = Random.Range(30, 50);
            Vector3 dir = Vector3.up;
            for (int i = 0; i < 330; i += angle)
            {
                dir = Quaternion.Euler(0, 0, angle) * dir;
                enemyFire(dir);
            }
        }
    }

    private void enemyFire(Vector3 dir)
    {
        GameObject shot = manager.getShot(false);
        shot.transform.position = transform.position + 2 * dir;
        shot.transform.up = dir;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.tag == "Shot")
        {
            this.gameObject.SetActive(false);
        }
    }

}
