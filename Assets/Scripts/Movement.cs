using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float zMove = 10;        
    Vector3 startPos;
    Vector3 endPos;
    // Start is called before the first frame update
    void Start()
    {
        
        startPos = transform.position;
        endPos = transform.position + new Vector3(0, 0, zMove);
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(startPos, endPos, Mathf.PingPong(Time.time / 2, 1));
    }
}
