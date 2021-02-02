using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    Camera mainCamera;
    GameObject player;
    public static Transform playerTransform;

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

    // Pills.
    private int pillsNum = 50;
    private GameObject[] pills;

    // Game over screen.
    private int depth;
    private GameObject gameOverCanvas;
    private TMP_Text depthScore;
    public bool gameIsOver;

    // Pause.
    public bool isPaused = false;
    private GameObject PauseCanvas;
    private TMP_Text curDepthScore;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting the time.
        Time.timeScale = 1.0f;

        // Initializing chain links.
        linkPool = new GameObject[linkPoolNum];
        for (int i = 0; i < linkPoolNum; i++)
        {
            linkPool[i] = Instantiate(linkPrefab);
            linkPool[i].SetActive(false);
        }
        currLinkIdx = -1;

        // Initializing shots.
        shotPool = new GameObject[shotPoolNum];
        for (int i = 0; i < shotPoolNum; i++)
        {
            shotPool[i] = Instantiate(shotPrefab);
            shotPool[i].SetActive(false);
        }
        currShotIdx = 0;

        // Initializing pills.
        pills = new GameObject[pillsNum];
        for(int i = 0; i < pillsNum; ++i)
        {
            GameObject pill = Instantiate(Resources.Load("Pill")) as GameObject;
            pill.SetActive(false);
            pills[i] = pill;
        }

        // Initializing game over screen.
        depth = 0;
        gameOverCanvas = GameObject.Find("GameOverCanvas");
        gameOverCanvas.GetComponent<Canvas>().planeDistance = 100f;
        depthScore = GameObject.Find("DepthScore").GetComponent<TMP_Text>();
        gameIsOver = false;

        // Initializing pause screen.
        PauseCanvas = GameObject.Find("PauseCanvas");
        PauseCanvas.GetComponent<Canvas>().planeDistance = 101f;
        curDepthScore = GameObject.Find("CurrentDepthScore").GetComponent<TMP_Text>();
        isPaused = false;

    player = GameObject.Find("Player");
        playerTransform = player.transform;
    }

    public GameObject getLink(bool isStart)
    {
        currLinkIdx++;
        if (currLinkIdx > linkPoolNum - 1)
        {
            currLinkIdx = 0;
        }
        GameObject link = linkPool[currLinkIdx];
        link.SetActive(true);
        if (isStart)
        {
            link.transform.Find("startLink").gameObject.SetActive(true);
        }
        
        return link;
    }

    public void resetLink(GameObject link)
    {
        link.GetComponent<SpringJoint2D>().connectedBody = null;
        link.GetComponent<Collider2D>().enabled = true;
        link.transform.Find("startLink").gameObject.SetActive(false);
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

    public void PositionPills(float x, float y, int pillAmount)
    {
        int curPillIdx = 0;
        for (int p = 0; p < pillsNum; ++p)
        {
            if(curPillIdx >= pillAmount)
            {
                break;
            }
            GameObject pill = pills[p];
            if (!pill.activeSelf)
            {
                float xPos = x + 0.15f * Mathf.Cos(2f * Mathf.PI * curPillIdx / (float)pillAmount);
                float yPos = y + 0.15f * Mathf.Sin(2f * Mathf.PI * curPillIdx / (float)pillAmount);
                pill.transform.position = new Vector3(xPos, yPos, 4);
                pill.SetActive(true);
                curPillIdx++;
            }
        }
    }

    void Update()
    {
    }
    

    public bool getIsNotPaused()
    {
        return !isPaused;
    }
    public void pauseGame()
    {
        if (!gameIsOver)
        {
            // Disabling game fancutalities.
            player.GetComponent<PlayerInputWrapper>().enabled = false;
            // Pausing.
            isPaused = true;
            Time.timeScale = 0;
            mainCamera.GetComponent<AudioSource>().Pause();
            // Menus.
            depth = (int)Mathf.Floor(mainCamera.transform.position.y);
            curDepthScore.text = "So far you Reached: " + (-1) * depth * 10 + "m";
            PauseCanvas.GetComponent<Canvas>().planeDistance = 0.5f;
        }
    }

    public void resumeGame()
    {
        // Enabling game fancutalities.
        player.GetComponent<PlayerInputWrapper>().enabled = true;
        // Resuming.
        isPaused = false;
        Time.timeScale = 1;
        mainCamera.GetComponent<AudioSource>().Play();
        // Menus.
        PauseCanvas.GetComponent<Canvas>().planeDistance = 100f;
    }

    public void gameOver()
    {
        if (!gameIsOver)
        {
            isPaused = true;
            Time.timeScale = 0.0f;
            depth = (int)Mathf.Floor(mainCamera.transform.position.y);
            depthScore.text = "You Reached: " + (-1) * depth * 10 + "m";
            gameOverCanvas.GetComponent<Canvas>().planeDistance = 0.5f;
        }
        gameIsOver = true;
    }
}
