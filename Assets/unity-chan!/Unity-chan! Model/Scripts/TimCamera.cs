using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimCamera : MonoBehaviour
{
    public float speed = 1.5f;
    private float X;
    private float Y;

    public float smooth = 10f;       // カメラモーションのスムーズ化用変数
    public Transform standardPos;          // the usual position for the camera, specified by a transform in the game
    bool bQuickSwitch = false;  //Change Camera Position Quickly


    void Start()
    {
        // 各参照の初期化
        //standardPos = GameObject.Find("FirstPerson").transform;


        //カメラをスタートする
        //transform.position = standardPos.position;
        //transform.forward = standardPos.forward;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            standardPos.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            //X = transform.rotation.eulerAngles.x;
            //Y = transform.rotation.eulerAngles.y;
            X = standardPos.rotation.eulerAngles.x;
            Y = standardPos.rotation.eulerAngles.y;

            //X = Mathf.Clamp(X, -90f, 90f);
            if(X>=80f && X<=90f)
            {
                X = 80f;
            }
            if (X >= 90f && X <= 100f)
            {
                X = 100f;
            }
            if (X >= 260f && X <= 270f)
            {
                X = 260f;
            }
            if (X >= 270f && X <= 280f)
            {
                X = 280f;
            }
            //Debug.Log(X);
            //transform.localRotation = Quaternion.Euler(X, Y, 0);
            //standardPos.Rotate(Vector3.up * X);
            standardPos.rotation = Quaternion.Euler(X, Y, 0);

            //standardPos.localRotation = Quaternion.Euler(0, Y, 0);
        }
    }
    void FixedUpdate()  // このカメラ切り替えはFixedUpdate()内でないと正常に動かない
    {
        setCameraPositionNormalView();
    }
    void setCameraPositionNormalView()
    {
        if (bQuickSwitch == false)
        {
            // the camera to standard position and direction
            transform.position = Vector3.Lerp(transform.position, standardPos.position, Time.fixedDeltaTime * smooth);
            transform.forward = Vector3.Lerp(transform.forward, standardPos.forward, Time.fixedDeltaTime * smooth);
        }
        else
        {
            // the camera to standard position and direction / Quick Change
            transform.position = standardPos.position;
            transform.forward = standardPos.forward;
            bQuickSwitch = false;
        }
    }
}
