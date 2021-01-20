using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerM2 : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 40f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        //Vector3 moveup = ;
        controller.Move(move * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.Space))
        {
            controller.Move(transform.up * 0.5f);
        }

        if (Input.GetKey(KeyCode.E))
        {
            controller.Move(transform.up * -0.5f);
        }
    }
}
