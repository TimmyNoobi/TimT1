using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithZoom : MonoBehaviour
{
    
    Transform TheCamera;
    // Start is called before the first frame update
    void Start()
    {
        TheCamera = Camera.main.transform;
    }
    float distance = 0;
    public float scaleoffset = 1f;
    float tscale = 1f;
    public float flip = 1f;
    public float tscalefactor = 150;
    // Update is called once per frame
    void Update()
    {
        //distance = Vector3.Distance(TheCamera.position, transform.position);
        //tscale = scaleoffset + distance / tscalefactor; 
        //transform.localScale = new Vector3(tscale * flip, tscale, tscale);
    }
    private void FixedUpdate()
    {
        distance = Vector3.Distance(TheCamera.position, transform.position);
        tscale = scaleoffset + distance / tscalefactor;
        transform.localScale = new Vector3(tscale * flip, tscale, tscale);
    }
}
