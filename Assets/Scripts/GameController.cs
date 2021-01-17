using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Image sloMoSign;
    public List<Image> hearts;
    private int currHearts = 3;


    // Link Object Pooling:
    public GameObject linkPrefab;
    private GameObject[] linkPool;
    public int linkPoolNum;
    private int currLinkIdx;

    // Shot Object Pooling:
    public GameObject shotPrefab;
    private GameObject[] shotPool;
    public int shotPoolNum;
    private int currShotIdx;

    // Start is called before the first frame update
    void Start()
    {
        sloMoSign.enabled = false;

        // Initializing chain links.
        linkPool = new GameObject[linkPoolNum];
        for (int i = 0; i < linkPoolNum; i++)
        {
            linkPool[i] = Instantiate(linkPrefab);
            //linkPool[i].transform.parent = this.transform;
            linkPool[i].SetActive(false);
        }
        currLinkIdx = 0;

        // Initializing shots.
        shotPool = new GameObject[shotPoolNum];
        for (int i = 0; i < shotPoolNum; i++)
        {
            shotPool[i] = Instantiate(shotPrefab);
            shotPool[i].SetActive(false);
        }
        currShotIdx = 0;
    }

    public GameObject getLink()
    {
        GameObject link = linkPool[currLinkIdx];
        currLinkIdx++;
        if (currLinkIdx > linkPoolNum - 1)
        {
            currLinkIdx = 0;
        }
        link.SetActive(true);
        
        return link;
    }

    public void resetLink(GameObject link)
    {
        link.GetComponent<SpringJoint2D>().connectedBody = null;
        link.GetComponent<Collider2D>().enabled = true;
        link.SetActive(false);
    }

    public GameObject getShot(bool isPlayer)
    {
        GameObject shot = shotPool[currShotIdx];
        shot.SetActive(true);
        ShotScript shotScript = shot.GetComponent<ShotScript>();
        currShotIdx++;
        if (currShotIdx > shotPoolNum - 1)
        {
            currShotIdx = 0;
        }
        shotScript.setUpShot(isPlayer);
        return shot;
    }

    public void sloMoStart()
    {
        sloMoSign.enabled = true;
    }
    public void sloMoEnd()
    {
        sloMoSign.enabled = false;
    }

    public void loseHeart()
    {
        if (currHearts == 0)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            hearts[currHearts - 1].enabled = false;
            currHearts--;
        }
    }

}
