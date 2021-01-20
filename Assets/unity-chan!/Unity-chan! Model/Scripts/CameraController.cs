//CameraController.cs for UnityChan
//Original Script is here:
//TAK-EMI / CameraController.cs
//https://gist.github.com/TAK-EMI/d67a13b6f73bed32075d
//https://twitter.com/TAK_EMI
//
//Revised by N.Kobayashi 2014/5/15 
//Change : To prevent rotation flips on XY plane, use Quaternion in cameraRotate()
//Change : Add the instrustion window
//Change : Add the operation for Mac
//




using UnityEngine;
using System.Collections;

namespace UnityChan
{
	enum MouseButtonDown
	{
		MBD_LEFT = 0,
		MBD_RIGHT,
		MBD_MIDDLE,
	};

	public class CameraController : MonoBehaviour
	{
        public float speed = 3.5f;
        private float X;
        private float Y;

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
                X = transform.rotation.eulerAngles.x;
                Y = transform.rotation.eulerAngles.y;
                transform.rotation = Quaternion.Euler(X, Y, 0);
            }
        }
    }
}
