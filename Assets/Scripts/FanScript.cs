using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanScript : MonoBehaviour
{
    private float angularSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        angularSpeed = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        if((gameObject.transform.eulerAngles.y > 30f && gameObject.transform.eulerAngles.y < 35f) && Mathf.Sign(angularSpeed) > 0)
        {
            angularSpeed = -30f;
        }
        if((gameObject.transform.eulerAngles.y < 330f && gameObject.transform.eulerAngles.y > 325f) && Mathf.Sign(angularSpeed) < 0)
        {
            angularSpeed = 30f;
        }
        gameObject.transform.eulerAngles += new Vector3(0, angularSpeed * Time.deltaTime, 0);
    }
}
