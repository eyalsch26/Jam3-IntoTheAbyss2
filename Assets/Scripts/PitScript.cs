using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Notes:
//      1. There are three difficulty: Easy(0), Medium(1) and Hard(2).
//      2. A Room consists of two Pits.

public class PitScript : MonoBehaviour
{
    // Camera.
    private Transform cam;
    
    // Constants.
    public static float pitWidth;
    public static float pitHeight;
    private int levelsNum = 3;
    private int roomsNum = 3;
    private float distanceToNextRoom = 31.24f;
    private float maxDistanceFromRoom= 7.81f;
    private float xPos = 0f;
    private float zPos = 0f;

    // Variables.
    private float pitPositionY;
    private int roomIndex;
    private bool atLastRoom;
    
    // Data sturctures.
    private GameObject firstRoom;
    private GameObject lastRoom;
    private GameObject[,] rooms; // Room == two pits. Levels x Rooms.
    private int[] activeRooms; // Levels x Rooms. The j'th room in the i'th difficulty is active == activeRooms[i] == j. Otherwise -1.

    private void Awake()
    {
        cam = Camera.main.transform;

        pitWidth = 6.03f; // Was 6.9f. 6.03 == 18 grid units.
        pitHeight = 15.41f; // Was 15.62f. 15.41 == 46 grid units.
        pitPositionY = 0f;
    }

    private void Start()
    {
        roomIndex = 0;
        atLastRoom = false;
        rooms = new GameObject[levelsNum, roomsNum];
        activeRooms = new int[levelsNum];

        // Initializes the pool of rooms accoriding to their level.
        for (int l = 0; l < levelsNum; ++l)
        {
            for (int r = 0; r < roomsNum; ++r)
            {
                GameObject room = Instantiate(Resources.Load("Difficulty" + l + "Room" + r)) as GameObject; // Instantiate(publicRooms[(l % levelsNum) * levelsNum + (r % roomsNum)]) as GameObject;
                room.SetActive(false);
                rooms[l, r] = room;
            }
            activeRooms[l] = -1; // Setting the rooms to not active.
        }
        // Positioning the first room.
        firstRoom = Instantiate(Resources.Load("FirstRoom")) as GameObject; ;
        firstRoom.transform.position = new Vector3(xPos, pitPositionY, zPos);
        firstRoom.SetActive(true);
        lastRoom = Instantiate(Resources.Load("LastRoom")) as GameObject; ;
        lastRoom.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(pitPositionY - cam.position.y > maxDistanceFromRoom && !atLastRoom)
        {
            pitPositionY -= distanceToNextRoom;
            PositionBackgrounds();
        }
    }

    private void PositionBackgrounds()
    {
        // Disabling the previous unseen rooms.
        for(int l = 0; l < levelsNum; ++l)
        {
            if(activeRooms[l] != -1)
            {
                GameObject curRoom = rooms[l, activeRooms[l]];
                if (curRoom.transform.position.y - cam.position.y > distanceToNextRoom)
                {
                    activeRooms[l] = -1;
                    curRoom.SetActive(false);
                }
            }
        }
        //// Choosing a level and a room to reposition.
        //int levelIdx = Random.Range(0, levelsNum - 1);
        //int roomIdx = Random.Range(0, roomsNum - 1);
        //while(activeRooms[levelIdx] == roomIdx)
        //{
        //    roomIdx = Random.Range(0, roomsNum - 1);
        //}
        int roomIdx = roomIndex % roomsNum;
        int levelIndex = roomIndex / levelsNum;
        activeRooms[levelIndex] = roomIdx;
        // Positioning the chosen room.
        if(roomIndex < levelsNum * roomsNum)
        {
            GameObject room = rooms[levelIndex, roomIdx];
            room.transform.position = new Vector3(xPos, pitPositionY, zPos);
            room.SetActive(true);
        }
        else
        {
            lastRoom.transform.position = new Vector3(xPos, pitPositionY, zPos);
            lastRoom.SetActive(true);
            atLastRoom = true;
        }
        roomIndex++;
    }
}
