using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillScript : MonoBehaviour
{
    private float angularSpeed;
    private float pullingRadius = 1.25f;
    private float playerGravity = 5f;
    public GameObject pillBody;
    public GameObject sparks;
    
    // Start is called before the first frame update
    void Start()
    {
        angularSpeed = 90f; // 90 degrees in a second.
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.eulerAngles += new Vector3(0, angularSpeed * Time.deltaTime, 0);
        PullToPlayer();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            StartCoroutine(flashDissapear());
        }
    }

    IEnumerator flashDissapear()
    {
        pillBody.SetActive(false);
        sparks.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        gameObject.SetActive(false);
        pillBody.SetActive(true);
        sparks.SetActive(false);
    }

    private void PullToPlayer()
    {
        Vector3 difference = GameController.playerTransform.position - gameObject.transform.position;
        float distance = difference.magnitude;
        if(distance < pullingRadius)
        {
            gameObject.transform.position = gameObject.transform.position + difference.normalized * playerGravity * (pullingRadius - distance) * Time.deltaTime;
        }
    }
}
