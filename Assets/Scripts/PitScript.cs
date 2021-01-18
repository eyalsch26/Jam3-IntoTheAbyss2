using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitScript : MonoBehaviour
{
    private Transform cam;
    public GameObject[] pits;
    public static float pitWidth;
    public static float pitHeight;

    private void Awake()
    {
        cam = Camera.main.transform;

        pitWidth = 6.03f; // Was 6.9f. 6.03 == 18 grid units.
        pitHeight = 15.41f; // Was 15.62f. 15.41 == 46 grid units.
    }

    // Update is called once per frame
    void Update()
    {
        PositionBackgrounds();
    }

    private void PositionBackgrounds()
    {
        for (int i = 0; i < pits.Length; ++i)
        {
            float curPosX = pits[i].transform.position.x;
            float curPosY = pits[i].transform.position.y;
            float curPosZ = pits[i].transform.position.z;
            float camBgDis = Mathf.Abs(cam.position.y - curPosY);
            if (camBgDis > 2f * 15.62f)
            {
                curPosY = pits[(i + 1) % pits.Length].transform.position.y - 2f * 15.62f;
                pits[i].transform.position = new Vector3(curPosX, curPosY, curPosZ);
            }
        }
    }
}
