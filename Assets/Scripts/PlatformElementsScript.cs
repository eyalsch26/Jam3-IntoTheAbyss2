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
    private float previousPitPosition;
    private float nextPitPosition;

    // Constants.
    private int platmormsNum = 5;
    private int platformSizeNum = 4;
    private int lasersNum = 2;
    //private int barrelsNum = 5;
    //private int enemiesNum = 5;
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
    public GameObject[] prefabPlatfroms;
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
        gridWidth = PitScript.pitWidth / gridUnit; // 18.
        gridHeight = PitScript.pitHeight / gridUnit; // 46.
        xPosRange = 0.5f * gridWidth;
        yPosRange = 0.5f * gridHeight;
        previousPitPosition = 15.62f;
        nextPitPosition = -15.62f;
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
        previousPlatforms = new GameObject[platmormsNum, platformSizeNum];
        curPlatforms = new GameObject[platmormsNum, platformSizeNum];
        nextPlatforms = new GameObject[platmormsNum, platformSizeNum];
        previousAcidTanks = new GameObject[platmormsNum, platformSizeNum];
        curAcidTanks = new GameObject[platmormsNum, platformSizeNum];
        nextAcidTanks = new GameObject[platmormsNum, platformSizeNum];
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
            
            for (int j = 0; j < platformSizeNum; ++j)
            {
                // Platforms.
                GameObject platform = Instantiate(prefabPlatfroms[j]) as GameObject; // Instantiate(Resources.Load("Platform" + (j + 2))) as GameObject;
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
            //barrels[i] = Instantiate(Resources.Load("Acid Barrel")) as GameObject;
            //centrifuges[i] = Instantiate(Resources.Load("Centrifuge")) as GameObject;
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
            if (previousPitPosition - mainCamTrans.position.y > maxPreviosPitDistance)
            {
                previousPitPosition -= 15.62f;
                nextPitPosition -= 15.62f;
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

            for (int j = 0; j < platformSizeNum; ++j)
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
            for (int j = 0; j < platformSizeNum; ++j)
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
            PositionLaser(l);
        }

        // Setting the position and the size of the platforms.
        for (int p = 0; p < platfomNum; ++p)
        {
            // Setting the size of the platform.
            int platformSize = Random.Range(0, platformSizeNum);
            
            // Setting the position.
            GameObject platform = nextPlatforms[p, platformSize];
            platform.SetActive(true);
            nextActivePlatforms[p] = platformSize;
            PositionPlatform(platform, p, platformSize);
        }

        // Setting the acid tanks on the platforms.
        for (int a = 0; a < acidTankNum; ++a)
        {
            GameObject acidTank = nextAcidTanks[a, nextActivePlatforms[a]];
            acidTank.SetActive(true);
            acidTank.transform.position = nextPlatforms[a, nextActivePlatforms[a]].transform.position + new Vector3(0, 0.31f, 0);
        }
    }

    // Finds an empty Y coordinate for a laser and positions it there.
    private void PositionLaser(int l)
    {
        int laserPos = Random.Range(1, (int)gridHeight - minLasersDistance); // Distance of 1 from pit's end to avoid adjacent lasers with the next pit.
                                                             // If there's already an element in that coordinate or distance to other laser is less than 1 try again.
        while (nextGrid[0, laserPos] != 0 || nextGrid[0, laserPos + 1] == 1 || nextGrid[0, laserPos - 1] == 1)
        {
            laserPos = Random.Range(0, (int)gridHeight);
        }
        nextGrid[9, laserPos] = 1;
        GameObject laser = nextLasers[l];
        laser.SetActive(true);
        laser.transform.position = new Vector3(0f, GridToWorldY(laserPos), elementsPosZ);
    }

    // Finds an empty coordinate for a platform and positions it there.
    private void PositionPlatform(GameObject platform, int p, int platformSize)
    {
        int x = Random.Range(0, (int)gridWidth);
        int y = Random.Range(0, (int)gridHeight);
        //bool valid = CheckPositionValidity(x, y, platformSize);
        //while (!valid) // There is an element in that coordinate.
        //{
        //    x = Random.Range(1, (int)gridWidth);
        //    y = Random.Range(0, (int)gridHeight);
        //    valid = CheckPositionValidity(x, y, platformSize);
        //}
        platform.transform.position = new Vector3(GridToWorldX(x, platformSize), GridToWorldY(y), elementsPosZ);
    }

    // Checks if the given coordinate is valid.
    // The conditions are:
    // 1. Don't overlap with other objects.
    // 2. Minimal distance to laser is 4.
    // 3. Minimal distacne to other platform on Y axis is 7 if overlap on X.
    // 4. Minimal distance to other platform on X axis is 4.
    // 5. Maximal platform number on the same level (Y axis) is 2.
    private bool CheckPositionValidity(int x, int y, int size)
    {
        // Overlap.
        bool overlap = false;
        bool yOverlap = false;
        // Hit the wall.
        bool inWall = (x - (size + 2)) < -1;
        // Close to laser.
        bool closeToLaser = false;
        // Number of platforms on the same level.
        int levelPlatforms = 0;
        bool twoPlatformsInLevel = false;
        for (int i = 0; i < gridWidth; ++i)
        {
            int otherContent = nextGrid[i, y];
            if (otherContent != 0 || Mathf.Abs(i - x) < minPlatformsDistanceX + 5)
            {
                overlap = true;
            }
            if (otherContent > 1)
            {
                levelPlatforms++;
            }
            for (int j = 1; j < minPlatformsDistanceY; ++j)
            {
                int curY = y - j;
                if (curY < 0)
                {
                    break;
                }
                otherContent = nextGrid[i, j];
                if (otherContent - size > Mathf.Abs(i - x))
                {
                    yOverlap = true;
                }
            }
        }
        twoPlatformsInLevel = levelPlatforms > 1;
        for (int k = -minDistLaserPlatform; k < minDistLaserPlatform + 1; ++k)
        {
            if(y + k < 0 || y + k > gridWidth)
            {
                continue;
            }
            if (nextGrid[9, y + k] == 1)
            {
                closeToLaser = true;
            }
        }

        return !overlap && !yOverlap && !inWall && !closeToLaser && !twoPlatformsInLevel;
    }

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
    // Recieves a coordinate on the grid's X axis and returns it's world position.
    private float GridToWorldX(int coordinate, int size)
    {
        return ((float)coordinate + 0.5f * (float)(size % 2) - xPosRange) * gridUnit;
    }

    // Recieves a coordinate on the grid's Y axis and returns it's world position.
    private float GridToWorldY(int coordinate)
    {
        return ((float)coordinate + 0.5f - yPosRange) * gridUnit + nextPitPosition;
    }

    // Recieves a world position and returns it's coordinate on the grid.
    private int ConvertWorldToGrid(float coordinate)
    {
        return (int)(((coordinate) / gridUnit) - 0.5f);
    }
}
