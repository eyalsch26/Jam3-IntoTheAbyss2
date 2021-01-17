using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasuresCheckScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector3 a = gameObject.GetComponent<MeshRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
