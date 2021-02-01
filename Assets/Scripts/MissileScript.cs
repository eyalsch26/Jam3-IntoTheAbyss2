using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    public bool isActive = false;
    public float speed;
    public GameObject missile;
    public GameObject explosion;
    public GameObject blaze;
    Transform pTransform;
    public LauncherScript daddy;
    Vector3 prevMove;

    public static HashSet<string> nonBlockingObjectTags = new HashSet<string> {
    "Laser", "Rope", "Iodine", "Health"};


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Vector3 dir = (pTransform.position - transform.position).normalized;
            Vector3 move = Time.deltaTime * speed * (0.5f * dir + 0.5f * prevMove);
            transform.position += move;
            transform.position = new Vector3(transform.position.x, transform.position.y, pTransform.position.z);
            prevMove = move;
            transform.right = move;
        }
    }

    public void getLaunched(Transform p)
    {
        pTransform = p;
        blaze.SetActive(true);
        isActive = true;
        prevMove = transform.right;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (nonBlockingObjectTags.Contains(collision.tag))
        {
            return;
        }
        StartCoroutine(exploded());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Hazard")
        {
            return;
        }
        StartCoroutine(exploded());
    }


    IEnumerator exploded()
    {
        missile.SetActive(false);
        blaze.SetActive(false);
        explosion.SetActive(true);
        explosion.GetComponent<ParticleSystem>().Play();
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        daddy.resetMissile();
        isActive = false;
    }

    public void resetMissile()
    {
        missile.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
    }

    public bool getActive()
    {
        return isActive;
    }
}
