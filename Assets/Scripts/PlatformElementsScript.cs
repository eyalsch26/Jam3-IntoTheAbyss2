using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformElementsScript : MonoBehaviour
{
    // Camera.
    Transform mainCamTrans; // Transform component of the main camera.

    private bool doneStart = false;

    // Grid.
    // Note: The top left of the grid is 0,0.
    private float gridUnit; // One gridUnit == 0.335 Unity unit. So the distance between two grid coordinates is 0.335 in world terms.
    private float gridWidth; // How many grid units in the width of the screen.
    private float gridHeight; // How many grid units in the height of the screen.
    private float xPosRange;
    private float yPosRange;

    // Pits.
    private float previosPitPosition;

    // Constants.
    private int pitsNum = 3;
    private int platmormsNum = 5;
    private int platformTypeNum = 4;
    private int lasersNum = 2;
    private int barrelsNum = 5;
    private int enemiesNum = 5;
    private int minPlatformsDistanceX = 4; // In grid units.
    private int minPlatformsDistanceY = 7; // In grid units.
    private int minDistLaserPlatform = 4; // In grid units.
    private int minLasersDistance = 1; // In grid units.
    private float maxPreviosPitDistance = 31.24f; // 15.62f * 2f
    private float elementsPosZ = 4f; // Platforms, lasers and acid tanks z position in world.

    // Probabilities.
    private float oneLaserProbability = 0.5f;
    private float twoLasersProbability = 0.25f;
    private float oneAcidTankProbability = 0.25f;
    private float twoAcidTanksProbability = 0.125f;
    private float onePlatformProbability = 0.15f;
    private float twoPlatformsProbability = 0.3f;
    private float threePlatformsProbability = 0.3f;

    // Data sturctures.
    private int[] previousActivePlatforms; // -1-Not active, 0..3-Type fo platform.
    private int[] curActivePlatforms; // -1-Not active, 0..3-Type fo platform.
    private int[] nextActivePlatforms; // -1-Not active, 0..3-Type fo platform.
    private int[,] previousGrid; // Positions in grid units. 0-Empty, 1-Laser, 2..5-Platforms.
    private int[,] curGrid; // Positions in grid units. 0-Empty, 1-Laser, 2..5-Platforms.
    private int[,] nextGrid; // Positions in grid units. 0-Empty, 1-Laser, 2..5-Platforms.
    private GameObject[,] previousPlatforms;
    private GameObject[,] curPlatforms;
    private GameObject[,] nextPlatforms;
    private GameObject[,] previousAcidTanks;
    private GameObject[,] curAcidTanks;
    private GameObject[,] nextAcidTanks;
    private GameObject[] previousLasers;
    private GameObject[] curLasers;
    private GameObject[] nextLasers;
    private GameObject[] barrels;
    private GameObject[] centrifuges;

    private void Awake()
    {
        mainCamTrans = Camera.main.transform;

        gridUnit = 0.335f;
        gridWidth = PitScript.pitWidth / gridUnit;
        gridHeight = PitScript.pitHeight / gridUnit;
        xPosRange = 0.5f * gridWidth;
        yPosRange = 0.5f * gridHeight;
        previosPitPosition = 15.62f;
    }

    // Start is called before the first frame update
    void Start()
    {
        previousActivePlatforms = new int[platmormsNum];
        curActivePlatforms = new int[platmormsNum];
        nextActivePlatforms = new int[platmormsNum];
        previousGrid = new int[(int)gridWidth, (int)gridHeight];
        curGrid = new int[(int)gridWidth, (int)gridHeight];
        nextGrid = new int[(int)gridWidth, (int)gridHeight];
        previousPlatforms = new GameObject[platmormsNum, platformTypeNum];
        curPlatforms = new GameObject[platmormsNum, platformTypeNum];
        nextPlatforms = new GameObject[platmormsNum, platformTypeNum];
        previousAcidTanks = new GameObject[platmormsNum, platformTypeNum];
        curAcidTanks = new GameObject[platmormsNum, platformTypeNum];
        nextAcidTanks = new GameObject[platmormsNum, platformTypeNum];
        previousLasers = new GameObject[lasersNum];
        curLasers = new GameObject[lasersNum];
        nextLasers = new GameObject[lasersNum];
        barrels = new GameObject[platmormsNum];
        centrifuges = new GameObject[platmormsNum];

        // Creates empty grids.
        previousGrid = CreateEmptyGrid();
        curGrid = CreateEmptyGrid();
        nextGrid = CreateEmptyGrid();

        // Creates a pool of lasers.
        for (int l = 0; l < lasersNum; ++l)
        {
            GameObject laser = Instantiate(Resources.Load("Laser")) as GameObject;
            laser.SetActive(false);
            previousLasers[l] = laser;
            curLasers[l] = laser;
            nextLasers[l] = laser;
        }

        // Creating a pool of platforms, acid tanks, acid barrels and centrifuges.
        for (int i = 0; i < platmormsNum; ++i)
        {
            previousActivePlatforms[i] = -1;
            curActivePlatforms[i] = -1;
            nextActivePlatforms[i] = -1;
            
            for (int j = 0; j < platformTypeNum; ++j)
            {
                // Platforms.
                GameObject platform = Instantiate(Resources.Load("Platform" + (j + 2))) as GameObject;
                platform.SetActive(false);
                previousPlatforms[i, j] = platform;
                curPlatforms[i, j] = platform;
                nextPlatforms[i, j] = platform;
                
                // Acid Tanks.
                GameObject acidTank = Instantiate(Resources.Load("AcidTank " + (j + 2))) as GameObject;
                acidTank.SetActive(false);
                previousAcidTanks[i, j] = acidTank;
                curAcidTanks[i, j] = acidTank;
                nextAcidTanks[i, j] = acidTank;
            }
            barrels[i] = Instantiate(Resources.Load("Acid Barrel")) as GameObject;
            centrifuges[i] = Instantiate(Resources.Load("Centrifuge")) as GameObject;
            //RecreatePlatform(i);
        }

        doneStart = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (doneStart)
        {
            // If pit is too far above reposition it.
            if (previosPitPosition - mainCamTrans.position.y > maxPreviosPitDistance)
            {
                // Transferring the grids by pointers and clearing the previous grid.
                previousGrid = curGrid;
                curGrid = nextGrid;
                nextGrid = CreateEmptyGrid();
                // Transferring the platfroms data structures by pointers and disabling the previous platforms.
                previousPlatforms = curPlatforms;
                previousActivePlatforms = curActivePlatforms;
                curPlatforms = nextPlatforms;
                curActivePlatforms = nextActivePlatforms;
                DisablePlatforms();
                // Transferring the pits elements by pointers and disabling the previous elements.
                previousLasers = curLasers;
                curLasers = nextLasers;
                DisableLasers();
                previousAcidTanks = curAcidTanks;
                curAcidTanks = nextAcidTanks;
                DisableAcidTanks();
                // Setting up the new grid as nextGrid.
                CreateNextPit();
            }
        }
    }

    // Creates an empty grid.
    private int[,] CreateEmptyGrid()
    {
        int[,] grid = new int[(int)gridWidth, (int)gridHeight];
        for (int x = 0; x < (int)gridWidth; ++x)
        {
            for (int y = 0; y < (int)gridHeight; ++y)
            {
                grid[x, y] = 0;
            }
        }
        return grid;
    }

    // Sets the platforms to disable.
    private void DisablePlatforms()
    {
        for (int i = 0; i < platmormsNum; ++i)
        {
            nextActivePlatforms[i] = -1;

            for (int j = 0; j < platformTypeNum; ++j)
            {
                nextPlatforms[i, j].SetActive(false);
            }
        }
    }

    // Disables the lasers of the previous pit.
    private void DisableLasers()
    {
        for (int l = 0; l < lasersNum; ++l)
        {
            nextLasers[l].SetActive(false);
        }
    }

    // Disables the acid tanks of the previous pit.
    private void DisableAcidTanks()
    {
        for (int i = 0; i < platmormsNum; ++i)
        {
            for (int j = 0; j < platformTypeNum; ++j)
            {
                nextAcidTanks[i, j].SetActive(false);
            }
        }
    }

    // Creates the next pit stucture: sets number of lasers, acid tanks and positions the platforms.
    private void CreateNextPit()
    {
        // Setting the number of lasers, acidTanks and platforms.
        int laserNum = SetLasersNumber();
        int acidTankNum = SetAcidTanksNumber();
        int platfomNum = SetPlatformsNumber(acidTankNum);

        // Setting the position of the lasers.
        for (int l = 0; l < laserNum; ++l)
        {
            int laserPos = Random.Range(0, (int)gridHeight - 1); // Distance of 1 from pit's end to avoid adjacent lasers with the next pit.
            // If there's already an element in that coordinate or distance to other laser is less than 1 try again.
            while (nextGrid[0, laserPos] != 0 || nextGrid[0, laserPos + 1] == 1 || nextGrid[0, laserPos - 1] == 1)
            {
                laserPos = Random.Range(0, (int)gridHeight);
            }
            nextGrid[(int)xPosRange , laserPos] = 1;
            GameObject laser = nextLasers[l];
            laser.SetActive(true);
            laser.transform.position = new Vector3(ConvertGridToWorld((int)xPosRange), ConvertGridToWorld(laserPos), elementsPosZ);
        }

        // Setting the position of the platforms.
        for (int p = 0; p < platfomNum; ++p)
        {
            // Setting the type of the platform.
            int platformType = Random.Range(0, platformTypeNum);

            GameObject platform = nextPlatforms[p, platformType];
            int platformPosX = Random.Range(0, (int)gridWidth - 1);
            int platformPosY = Random.Range(0, (int)gridHeight - 1);
            // If there's already an element in that coordinate or distance to other laser is less than 1 try again.
            while (nextGrid[0, platformPosY] != 0 || nextGrid[0, platformPosY + 1] == 1 || nextGrid[0, platformPosY - 1] == 1)
            {
                platformPosY = Random.Range(0, (int)gridHeight);
            }
            nextGrid[(int)xPosRange, platformPosY] = 1;
            GameObject laser = nextLasers[p];
            laser.SetActive(true);
            laser.transform.position = new Vector3(ConvertGridToWorld((int)xPosRange), ConvertGridToWorld(platformPosY), elementsPosZ);
        }
    }

    // Finds an empty y coordinate for a laser.
    private int PositionLaser()
    {
        int y = Random.Range(0, (int)gridHeight);
        while (nextGrid[0, y] != 0) // There is an element in that coordinate.
        {
            y = Random.Range(0, (int)gridHeight);
        }
        nextGrid[0, y] = 1;
        return y;
    }

    // Repositioning and setting the charactaristics of the platform. Attributes are randomly generated.
    //private void RecreatePlatform(int platformIdx)
    //{
    //    // Size.
    //    platforms[platformIdx, activePlatforms[platformIdx]].SetActive(false); // Disabling the current platform.
    //    int size = Random.Range(0, platformTypeNum); // Choosing a new platform size.
    //    float platformBricks = (float)(size + 2);
    //    activePlatforms[platformIdx] = size;
    //    platforms[platformIdx, size].SetActive(true); // Enabling the new platform.

    //    // Platform type.
    //    int platformType = Random.Range(0, 4); // 0 - Empty, 1 - Accid barrel, 2 - Enemy, 3 - AcidTank.

    //    // Repositioning.
    //    float xPos = xPosRange - platformBricks;
    //    xPos = 0.5f * Random.Range((int)-xPosRange, (int)xPosRange + 1);
    //    float yPos = mainCamTrans.position.y - (ParallaxScript.frameHeight + Random.Range(0f, yPosRange) + platformIdx * yPosRange);
    //    platforms[platformIdx, size].transform.position = new Vector3(xPos, yPos, -1f);

    //    // Cleaning the platform.
    //    barrels[platformIdx].SetActive(false);
    //    centrifuges[platformIdx].SetActive(false);

    //    if (platformType == 1)
    //    {

    //        GameObject barrel = barrels[platformIdx];
    //        barrel.SetActive(true);
    //        float barrelPosX = xPos + platforms[platformIdx, size].transform.localScale.x * Random.Range(-0.75f, 0.75f);
    //        float BarrelPosY = yPos + 2 * platforms[platformIdx, size].transform.localScale.y + barrel.transform.localScale.y;
    //        barrel.transform.position = new Vector3(barrelPosX, BarrelPosY, -1);
    //    }
    //    else if (platformType == 2)
    //    {

    //        GameObject centrifuge = centrifuges[platformIdx];
    //        centrifuge.SetActive(true);
    //        float centrifugePosX = xPos + platforms[platformIdx, size].transform.localScale.x * Random.Range(-0.75f, 0.75f);
    //        float centrifugePosy = yPos + platforms[platformIdx, size].transform.localScale.y + centrifuge.transform.localScale.y;
    //        centrifuge.transform.position = new Vector3(centrifugePosX, centrifugePosy, -1);
    //    }
    //}

    // Returns the number of lasers to appear in the frame.
    // 0.25 - No lasers, 0.5 - One laser, 0.25 - Two lasers.
    private int SetLasersNumber()
    {
        float laserNum = Random.Range(0f, 1f);
        if(laserNum < oneLaserProbability)
        {
            return 1;
        }
        else if (laserNum < oneLaserProbability + twoLasersProbability)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    // Returns the number of acidTanks to appear in the frame.
    // 0.25 - No lasers, 0.5 - One laser, 0.25 - Two lasers.
    private int SetAcidTanksNumber()
    {
        float acidTankNum = Random.Range(0f, 1f);
        if (acidTankNum < oneAcidTankProbability)
        {
            return 1;
        }
        else if (acidTankNum < oneAcidTankProbability + twoAcidTanksProbability)
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    private int SetPlatformsNumber(int acidTanksNum)
    {
        float platformNum = Random.Range(0f, 1f);
        if (platformNum < onePlatformProbability)
        {
            return 1 + acidTanksNum;
        }
        else if (platformNum < onePlatformProbability + twoPlatformsProbability)
        {
            return 2 + acidTanksNum;
        }
        else if (platformNum < twoPlatformsProbability + threePlatformsProbability)
        {
            return 3 + acidTanksNum;
        }
        else
        {
            return 0 + acidTanksNum;
        }
    }

    // TODO: Fix mapping.
    // Recieves a coordinate on the grid and returns it's world position.
    private float ConvertGridToWorld(int coordinate)
    {
        return ((float)coordinate + 0.5f) * gridUnit;
    }

    // Recieves a world position and returns it's coordinate on the grid.
    private int ConvertWorldToGrid(float coordinate)
    {
        return (int)(((coordinate) / gridUnit) - 0.5f);
    }
}
